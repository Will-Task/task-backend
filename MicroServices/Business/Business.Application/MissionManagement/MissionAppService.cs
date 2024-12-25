using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Business.Enums;
using Business.FileManagement;
using Business.FileManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;
using Business.Permissions;
using Business.Specifications;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Specifications;

namespace Business.MissionManagement;

[Authorize(BusinessPermissions.TaskItem.Default)]
[RemoteService(false)]
public class MissionAppService : ApplicationService, IMissionAppService
{
    private (
        IRepository<Mission, Guid> Mission,
        IRepository<MissionI18N, Guid> MissionI18N,
        IRepository<MissionView> MissionView,
        IRepository<MissionCategory, Guid> MissionCategory,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18N,
        IRepository<MissionCategoryView> MissionCategoryView,
        IRepository<AbpUserView> AbpUserView,
        IRepository<LocalizationText> LocalizationText,
        IRepository<Language> Language
        ) _repositoys;

    private readonly IFileAppService _fileAppService;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<MissionAppService> _logger;
    private readonly IDataFilter _dataFilter;
    private readonly int ExcelBeginLine = 5;
    private readonly int ExcelEndLine = 14;

    // 設定定時任務最大數量
    private readonly int maxScheduleCount = 10;

    // 根據MissionImportDto除去匯入不需要的欄位
    private readonly List<string> ImportNotIncluded = new List<string>
    {
        "Id", "MissionFinishTime", "UserId", "TeamId", "ParentMissionId", "MissionCategoryId", "Lang"
    };

    public MissionAppService(IRepository<Mission, Guid> Mission,
        IRepository<MissionI18N, Guid> MissionI18N,
        IRepository<MissionView> MissionView,
        IRepository<MissionCategory, Guid> MissionCategory,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18N,
        IRepository<MissionCategoryView> MissionCategoryView,
        IRepository<AbpUserView> AbpUserView,
        IRepository<LocalizationText> LocalizationText,
        IRepository<Language> Language,
        IFileAppService fileAppService,
        IConfiguration Configuration,
        IDataFilter dataFilter,
        ILogger<MissionAppService> logger)
    {
        _repositoys = (Mission, MissionI18N, MissionView, MissionCategory, MissionCategoryI18N,
            MissionCategoryView, AbpUserView, LocalizationText, Language);
        _fileAppService = fileAppService;
        _Configuration = Configuration;
        _dataFilter = dataFilter;
        _logger = logger;
    }

    #region CRUD方法

    /// <summary>
    /// 新增/修改任務
    /// </summary>
    public async Task<MissionI18NDto> DataPost(CreateOrUpdateMissionDto input)
    {
        var newI18N = ObjectMapper.Map<CreateOrUpdateMissionDto, MissionI18N>(input);

        // 判斷修改(對現有任務增加別的語系)或新增
        if (input.Id.HasValue)
        {
            newI18N.MissionId = input.Id.Value;
            var mission = await _repositoys.Mission.GetAsync(input.Id.Value);
            var missionI18Ns = mission.MissionI18Ns;
            var missionI18N = missionI18Ns.FirstOrDefault(mn => mn.MissionId == input.Id && mn.Lang == input.Lang);

            // 判斷同個任務同個語系是否存在
            // I18N存在 => 修改現有語系資料
            if (missionI18N != null)
            {
                Guid nowId = input.Id.Value;
                // 定時任務新增
                if (mission.Schedule != input.Schedule)
                {
                    input.ScheduleMissionId = mission.ScheduleMissionId;
                    // 和原狀態不相同才須設定定時
                    await CreateTaskSchedule(input);
                    input.Id = nowId;
                }

                // 更新資料
                missionI18N.MissionName = input.MissionName;
                missionI18N.MissionDescription = input.MissionDescription;
                ObjectMapper.Map(input, mission);
            }
            // 不存在 => 增加現有任務的語系
            else
            {
                mission.AddMissionI18N(newI18N);
            }

            await _repositoys.Mission.UpdateAsync(mission);
        }
        // 新增任務
        else
        {
            input.Id = GuidGenerator.Create();
            var newMission = ObjectMapper.Map<CreateOrUpdateMissionDto, Mission>(input);
            newMission.UserId = CurrentUser.Id;
            newMission.TeamId = input.TeamId;
            newMission.AddMissionI18N(newI18N);

            // 定時任務新增
            if (input.Schedule.HasValue)
            {
                input.ScheduleMissionId = newMission.Id;
                await CreateTaskSchedule(input);
            }

            await _repositoys.Mission.InsertAsync(newMission);
        }

        return ObjectMapper.Map<CreateOrUpdateMissionDto, MissionI18NDto>(input);
    }

    /// <summary>
    /// 刪除任務(單 or 多筆)
    /// </summary>
    public async Task Delete(Guid id, int lang)
    {
        // 透過missionId和lang共同判斷要刪除的I18N
        await _repositoys.MissionI18N.DeleteAsync(new MissionI18NSpecification(id, lang), autoSave: true);
        // 若該任務不存在任何語系則刪除任務本體
        var count = await _repositoys.MissionI18N.CountAsync(new MissionI18NSpecification(id));
        if (count == 0)
        {
            await _repositoys.Mission.DeleteAsync(id);
        }
    }

    /// <summary>
    /// 查詢特定任務(單個)
    /// </summary>
    public async Task<MissionViewDto> Get(Guid id)
    {
        // 1. 撈特定類別的任務(直接透過sql中的view抓)
        var mission = await _repositoys.MissionView.GetAsync(new MissionSpecification(id));
        var dto = ObjectMapper.Map<MissionView, MissionViewDto>(mission);
        dto.ParentCategoryId =
            (await _repositoys.MissionCategory.GetAsync(mission.MissionCategoryId, includeDetails: false)).ParentId;
        return dto;
    }

    /// <summary>
    /// 查詢所有父任務(多個)
    /// </summary>
    public async Task<PagedResultDto<MissionViewDto>> GetAll(int page, int pageSize, bool allData,
        Guid? teamId, Guid? categoryId, Guid? parentId)
    {
        // 1. 抓該當前使用者所有mission
        var currentUserId = CurrentUser.Id;
        var query = await _repositoys.MissionView.GetQueryableAsync();
        // (當前使用者or所屬team) 且當前的任務類別且當前的parentId
        query = query.Where(new TeamOrUserMissionSpecification(teamId, currentUserId)
            .And(new ParentMissionSpecification(parentId))
            .And(new CategoryMissionSpecification(categoryId.HasValue ? categoryId : null)).ToExpression()
        );
        var count = await query.CountAsync();
        // 拿全部or分頁
        var parents = allData
            ? await query.ToListAsync()
            : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var dtos = ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(parents);
        dtos = allData ? dtos : dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var queryMission = await _repositoys.MissionI18N.GetQueryableAsync();
        
        // 指定語系 -> 中文 -> 任一語系 ， 符合規則的第一筆
        var defaultMission = queryMission.OrderBy(x => x.Lang == 1 ? 0 : x.Lang)
            .OrderBy(x => x.Lang).GroupBy(x => x.MissionId)
            .ToDictionary(g => g.Key, x => x.First());
            
        foreach (var dto in dtos)
        {
            if (dto.MissionName.IsNullOrEmpty())
            {
                dto.MissionName = defaultMission[dto.MissionId].MissionName;
            }

            if (dto.MissionDescription.IsNullOrEmpty())
            {
                dto.MissionDescription = defaultMission[dto.MissionId].MissionDescription;
            }
            dto.AttachmentCount = await _fileAppService.GetAttachmentCount(dto.MissionId);
        }
        return new PagedResultDto<MissionViewDto>(count, dtos);
    }

    /// <summary>
    /// 查詢特定類別任務總攬
    /// </summary>
    /// <param name="categoryId">任務子類別 Id</param>
    public async Task<List<MissionOverviewDto>> GetOverview(Guid categoryId, Guid? teamId)
    {
        var currentUserId = CurrentUser.Id;
        var dtos = new List<MissionOverviewDto>();
        var queryMission = await _repositoys.MissionView.GetQueryableAsync();
        /// 預設多國語系資料
        var missionMap = queryMission.OrderBy(x => x.Lang == 1 ? 0 : x.Lang).OrderBy(x => x.Lang)
            .GroupBy(x => x.MissionId).ToDictionary(g => g.Key, x => x.First().MissionName);
        /// (當前使用者or所屬team) 且當前的任務類別
        queryMission = queryMission.Where(new TeamOrUserMissionSpecification(teamId, currentUserId)
            .And(new CategoryMissionSpecification(categoryId)).ToExpression()
        );
        var parents = await queryMission.Where(x => x.ParentMissionId == null).ToListAsync();
        var subMap = queryMission.Where(x => x.ParentMissionId != null)
            .GroupBy(x => x.ParentMissionId)
            .ToDictionary(x => x.Key);

        foreach (var parent in parents)
        {
            var dto = new MissionOverviewDto();
            ObjectMapper.Map(parent, dto);
            if (dto.MissionName.IsNullOrEmpty())
            {
                dto.MissionName = missionMap[dto.MissionId];
            }
            /// 父任務是否有子任務檢查
            if (subMap.ContainsKey(parent.MissionId))
            {
                var subs = subMap[parent.MissionId].Where(x => x.Lang == parent.Lang).ToList();
                var subDtos = ObjectMapper.Map<List<MissionView>, List<SubMissionOverviewDto>>(subs);
                subDtos.ForEach(sub =>
                {
                    if (sub.MissionName.IsNullOrEmpty())
                    {
                        sub.MissionName = missionMap[sub.MissionId];
                    }
                });
                dto.SubMissions = subDtos;
            }
            dto.Lang = parent.Lang;
            dtos.Add(dto);
        }

        return dtos;
    }

    #endregion CRUD方法

    /// <summary>
    /// 任務提醒通知(寄email)
    /// </summary>
    [AllowAnonymous]
    public async Task MissionReminder()
    {
        var missions = await _repositoys.Mission.GetListAsync();
        var queryUser = await _repositoys.AbpUserView.GetQueryableAsync();
        var emailMap = queryUser.ToDictionary(x => x.Id, x => x.Email);

        foreach (var mission in missions.Where(x => x.MissionBeforeEnd.HasValue))
        {
            var now = Clock.Now;
            now = now.AddHours(mission.MissionBeforeEnd.Value);
            // 還沒再提醒的範圍時間內
            if (now < mission.MissionEndTime)
            {
                continue;
            }

            // var user = await _UserAppService.Get(mission.UserId.Value);
            // var user = await _userManager.FindByIdAsync(mission.UserId.ToString());
            // var query = await _UserRepository.GetQueryableAsync();
            // var email = await query.Select(u => u.Email).FirstAsync();
            await SendEmail("任務提醒通知", "你的任務快到期了喔，趕緊完成!", emailMap[mission.UserId.Value], null);
        }
    }

    /// <summary>
    /// 設置任務提醒時間(結束時間多久前)
    /// </summary>
    public async Task SetRemindTime(Guid id, int hour)
    {
        var mission = await _repositoys.Mission.GetAsync(id);
        mission.MissionBeforeEnd = hour;
    }


    /// <summary>
    /// 變更任務狀態
    /// </summary>
    public async Task UpdateMissionState(MissionFormData formData)
    {
        var mission = await _repositoys.Mission.GetAsync(formData.missionId);
        mission.MissionState = (MissionState)formData.state;
        mission.MissionFinishTime = (MissionState)formData.state == MissionState.IN_PROCESS ? null : Clock.Now;
    }

    private List<string> ExcelKeys = new List<string>
    {
        "MissionName", "MissionCategoryName", "MissionPriority",
        "MissionState", "MissionStartTime", "MissionEndTime"
    };

    /// <summary>
    /// 範本下載
    /// </summary>
    public async Task<BlobDto> DNSample(Guid parentId, string code)
    {
        var template = await _repositoys.LocalizationText.GetAsync(x => x.LanguageCode == code && x.Category == "Template" && x.ItemKey == "1");
        var blobDto = await _fileAppService.DNFile(template.ItemValue);
        using var memoryStream = new MemoryStream(blobDto.Content);
        using var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);
        worksheet.Name = blobDto.Name;
        var language = await _repositoys.Language.GetAsync(x => x.Code == code);

        var missionColumn = 'A';
        var categoryColumn = 'C';
        var startTimeColumn = 'D';
        var endTimeColumn = 'E';
        var remindTimeColumn = 'F';
        var priorityColumn = 'G';
        var stateColumn = 'H';
        var parentIdColumn = 'I';

        // 父類別資訊取得
        var queryMission = await _repositoys.MissionView.GetQueryableAsync();
        var parentMission = await queryMission
            .Where(x => x.MissionId == parentId && x.Lang == language.Id)
            .FirstAsync();
        var parentMissionDto = ObjectMapper.Map<MissionView, MissionImportDto>(parentMission);
        var properties = typeof(MissionImportDto).GetProperties();

        // 父類別資訊設定
        var nextChar = 'A';
        foreach (var property in properties.Where(x => !ImportNotIncluded.Contains(x.Name)))
        {
            worksheet.Cell($"{nextChar++}{ExcelBeginLine - 2}").Value = $"{property.GetValue(parentMissionDto)}";
        }

        // 類別設定
        var categoryRange = worksheet.Range($"{categoryColumn}{ExcelBeginLine}:{categoryColumn}{ExcelEndLine}");
        foreach (var cell in categoryRange.Cells())
        {
            cell.Value = $"{parentMission.MissionCategoryName}";
        }

        // 日期格式設定
        var rangeDate = worksheet.Range($"{startTimeColumn}{ExcelBeginLine}:{endTimeColumn}{ExcelEndLine}");
        foreach (var cell in rangeDate.Cells())
        {
            cell.Style.NumberFormat.Format = "yyyy/m/d h:mm:ss";
        }

        // TODO
        // 任務提醒時間下拉選單設定
        var remindTime = new List<string> { "", "24", "16", "6", "3", "1" };
        SetExcelFormattion(ref worksheet, remindTime, remindTimeColumn);

        // 任務優先度下拉選單設定(1~5)
        var priority = new List<string> { "1", "2", "3", "4", "5" };
        SetExcelFormattion(ref worksheet, priority, priorityColumn);

        // 任務狀態下拉選單設定
        var state = new List<string> { "TODO", "IN_PROCESS", "COMPLETED" };
        SetExcelFormattion(ref worksheet, state, stateColumn);

        // 設置固定區塊為唯讀
        // 全鎖
        worksheet.Protect();
        // 指定區塊解鎖
        worksheet.Range($"{missionColumn}{ExcelBeginLine}:B{ExcelEndLine}").Style.Protection
            .SetLocked(false);
        worksheet.Range($"{startTimeColumn}{ExcelBeginLine}:{stateColumn}{ExcelEndLine}").Style.Protection
            .SetLocked(false);

        // 寫入父任務missionId
        var range = worksheet.Range($"{parentIdColumn}{ExcelBeginLine}:{parentIdColumn}{ExcelEndLine}");
        foreach (var cell in range.Cells())
        {
            cell.Value = $"{parentId.ToString()}";
        }

        worksheet.Column($"{parentIdColumn}").Hide();

        using var savingMemoryStream = new MemoryStream();
        workbook.SaveAs(savingMemoryStream);
        blobDto.Content = savingMemoryStream.ToArray();
        blobDto.Name = $"{parentMission.MissionName}的下載範本.xlsx";
        return blobDto;
    }

    /// <summary>
    /// 資料匯入檢查
    /// </summary>
    public async Task<List<MissionImportDto>> ImportFileCheck(Guid parentId, Guid? teamId, string code, IFormFile file)
    {
        var currentUserId = CurrentUser.Id;
        var dtos = new List<MissionImportDto>();
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        using var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);
        var missionColumn = 'A';
        var parentIdColumn = 'I';
        var langugage = await _repositoys.Language.GetAsync(x => x.Code == code);

        // 從excel取出資料
        for (int i = ExcelBeginLine; i <= ExcelEndLine; i++)
        {
            if (Guid.TryParse(worksheet.Cell($"{parentIdColumn}{i}").GetString(), out var pId) && pId != parentId)
            {
                throw new BusinessException("範本不符合當前父任務");
            }

            // row沒被填 -> 直接結束
            if (worksheet.Cell($"{missionColumn}{i}").IsEmpty())
            {
                break;
            }

            var dto = new MissionImportDto();
            dto.UserId = currentUserId;
            dto.TeamId = teamId;
            dto.ParentMissionId = parentId;
            dto.Lang = langugage.Id;
            var queryMission = await _repositoys.Mission.GetQueryableAsync();
            var categoryId = await queryMission.Where(x => x.Id == parentId)
                .Select(x => x.MissionCategoryId).FirstAsync();
            dto.MissionCategoryId = categoryId;

            var nextchar = 'A';
            // 根據excelKey對物件賦值
            var properties = dto.GetType().GetProperties();
            foreach (var property in properties
                         .Where(x => !ImportNotIncluded.Contains(x.Name)))
            {
                var value = worksheet.Cell($"{nextchar}{i}").GetString();
                if (value.IsNullOrEmpty())
                {
                    nextchar++;
                    continue;
                }

                if (property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(MissionState) ||
                    property.PropertyType == typeof(int?))
                {
                    MissionState.TryParse<MissionState>(value, out var state);
                    property.SetValue(dto, Convert.ToInt32(state));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    string format = "yyyy/M/d tt h:mm:ss";
                    var culture = new CultureInfo("zh-TW");
                    DateTime.TryParseExact(value, format, culture,
                        DateTimeStyles.AllowWhiteSpaces, out DateTime dateTime);
                    property.SetValue(dto, dateTime);
                }
                else if(!value.IsNullOrEmpty())
                {
                    property.SetValue(dto, value);
                }

                nextchar++;
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    /// <summary>
    /// 資料匯入
    /// </summary>
    public async Task ImportFile(List<MissionImportDto> dtos)
    {
        var missions = new List<Mission>();

        // 將讀出資料進行匯入
        foreach (var dto in dtos)
        {
            var mission = ObjectMapper.Map<MissionImportDto, Mission>(dto);
            mission.MissionI18Ns = new List<MissionI18N>();
            var i18NDto = new MissionI18NDto
            {
                Id = GuidGenerator.Create(),
                MissionName = dto.MissionName,
                MissionDescription = dto.MissionDescription,
                Lang = dto.Lang
            };
            mission.MissionI18Ns.Add(ObjectMapper.Map<MissionI18NDto, MissionI18N>(i18NDto));
            missions.Add(mission);
        }

        await _repositoys.Mission.InsertManyAsync(missions, autoSave: true);
    }

    /// <summary>
    /// 資料匯出(子任務)
    /// </summary>
    public async Task<MyFileInfoDto> ExportFile(Guid parentId, string code)
    {
        // 取得欲寫入的範本檔案
        var file = await _repositoys.LocalizationText.GetAsync(x => x.LanguageCode == code && x.Category == "Template" && x.ItemKey == "2");
        var blobDto = await _fileAppService.DNFile(file.ItemValue);
        using var memoryStream = new MemoryStream(blobDto.Content);
        using var workBook = new XLWorkbook(memoryStream);
        var language = await _repositoys.Language.GetAsync(x => x.Code == code);

        // 取得父任務
        var parent = await _repositoys.MissionView.FirstAsync(x =>
            x.MissionId == parentId && x.Lang == language.Id);
        var parentExportDto = ObjectMapper.Map<MissionView, MissionExportDto>(parent);
        // 取得parentId的子任務
        var subs = await _repositoys.MissionView.GetListAsync(x =>
            x.ParentMissionId == parentId && x.Lang == language.Id);
        var subExportDtos = ObjectMapper.Map<List<MissionView>, List<MissionExportDto>>(subs);
        int start = 3;
        var nextChar = 'A';

        var worksheet = workBook.Worksheet(1);
        worksheet.Name = parent.MissionName;

        var propertys = typeof(MissionExportDto).GetProperties();
        foreach (var property in propertys)
        {
            worksheet.Cell($"{nextChar++}{start}").Value = $"{property.GetValue(parentExportDto)}";
        }

        start += 2;

        // 資料寫入檔案
        foreach (var dto in subExportDtos)
        {
            nextChar = 'A';
            foreach (var property in propertys)
            {
                worksheet.Cell($"{nextChar++}{start}").Value = $"{property.GetValue(dto)}";
            }

            start++;
        }

        using var savingMemoryStream = new MemoryStream();
        workBook.SaveAs(savingMemoryStream);
        return new MyFileInfoDto
        { FileContent = savingMemoryStream.ToArray(), FileName = $"{parent.MissionName}的匯出檔案.xlsx" };
    }

    /// <summary>
    /// email寄送
    /// </summary>
    [AllowAnonymous]
    private async Task SendEmail(string subject, string body, string email, MyFileInfoDto fileInfoDto = null)
    {
        // 為了讓mailMessage中的附件stream可以被釋放
        using var mailMessage = new MailMessage()
        {
            Subject = subject,
            Body = body,
            From = new MailAddress(_Configuration["Email:UserName"]),
            To = { email },
        };

        if (fileInfoDto != null)
        {
            _logger.LogInformation("=======================檔案不為空，寄送每周報告====================================");
            // SMTP 客戶端實際上是在方法結束後才開始讀取附件的。這邊不能使用using
            var memoryStream = new MemoryStream(fileInfoDto.FileContent);
            // 需指定media-type，並在檔案名稱後面加上.xlsx副檔名
            mailMessage.Attachments.Add(
                new Attachment(memoryStream, fileInfoDto.FileName + ".xlsx", "application/xlsx"));
        }

        var client = new SmtpClient();
        client.Host = "smtp.gmail.com";
        client.Port = 587;
        client.UseDefaultCredentials = false;
        // 密碼是應用程式密碼
        client.Credentials =
            new NetworkCredential(_Configuration["Email:UserName"], _Configuration["Email:Password"]);
        client.EnableSsl = true;
        await client.SendMailAsync(mailMessage);
        client.Dispose();
    }

    /// <summary>
    /// 任務即將到期(根據設定時間)通知
    /// </summary>
    [AllowAnonymous]
    public async Task IdentifyExpiringTasks()
    {
        var now = Clock.Now;
        var query = await _repositoys.MissionView.GetQueryableAsync();
        // 判斷結束時間和提醒時間差距
        var missionMap = query.Where(m =>
                m.MissionEndTime <= now.AddHours(m.MissionBeforeEnd.Value) && m.MissionFinishTime == null)
            .GroupBy(g => g.MissionId).ToDictionary(g => g.Key, g => g.First());
        var queryUser = await _repositoys.AbpUserView.GetQueryableAsync();
        var emailMap = queryUser.ToDictionary(x => x.Id, x => x.Email);
        
        foreach (var mission in missionMap)
        {
            var misssionName = mission.Value.MissionName;
            // 因為可能 Mission 中 isActive = false 的會找不到
            if (misssionName.IsNullOrEmpty())
            {
                continue;
            }

            // 寄發email(之後放入使用者email)
            await SendEmail($"{misssionName}任務在{mission.Value.MissionBeforeEnd}小時內即將到期", "任務快過期了喔，趕緊去完成",
                emailMap[mission.Value.UserId.Value]);
        }
    }

    /// <summary>
    /// 任務報告產出Excel(1 week -> 7 days)傳送到email
    /// </summary>
    [AllowAnonymous]
    public async Task ExportReport()
    {
        // 排程設置一個禮拜執行一次，並且固定禮拜一執行
        var now = Clock.Now;
        var sevenDayBefore = now.AddDays(-7);

        var exportKeys = new List<string>
        {
            "MissionCategoryName", "MissionStartTime", "MissionEndTime",
            "MissionFinishTime", "MissionName", "MissionDescription"
        };

        // 過期 -> 期限內未完成 ， 輸出報告也需顯示
        // 拿到輸入的報告範本
        var blobDto = await _fileAppService.DNFile("每周輸出報告.xlsx");
        using var memoryStream = new MemoryStream(blobDto.Content);
        using var workBook = new XLWorkbook(memoryStream);
        var worksheet = workBook.Worksheet(1);

        // 根據不同子任務寫不同tab中，tab以父任務名稱為主
        var query = await _repositoys.MissionView.GetQueryableAsync();
        // key : userId , value : subMissions
        var submissions = query.Where(m => m.ParentMissionId != null && m.UserId != null)
            .GroupBy(m => m.UserId)
            .ToDictionary(m => m.Key, m => m.ToList());
        
        var queryUser = await _repositoys.AbpUserView.GetQueryableAsync();
        var emailMap = queryUser.ToDictionary(x => x.Id, x => x.Email);
        
        foreach (var key in submissions.Keys)
        {
            int i = ExcelBeginLine;

            foreach (var submission in submissions[key])
            {
                // 判斷是否在一周內(結束日期或是完成日期在這周)
                if (sevenDayBefore > submission.MissionEndTime ||
                    (submission.MissionFinishTime.HasValue && sevenDayBefore > submission.MissionFinishTime))
                {
                    continue;
                }

                var tabName = "";
                var parentMissionI18N =
                    await _repositoys.MissionI18N.FirstOrDefaultAsync(m => m.Id == submission.ParentMissionId);
                if (parentMissionI18N == null)
                {
                    tabName = "empty";
                }
                else
                {
                    tabName = parentMissionI18N.MissionName;
                }

                worksheet.Name = tabName;
                var nextChar = 'A';
                foreach (var exportKey in exportKeys)
                {
                    var propertyInfo = submission.GetType().GetProperty(exportKey);
                    worksheet.Cell($"{nextChar}{i}").Value = $"{propertyInfo.GetValue(submission)}";
                    nextChar++;
                }
                
                i++;
            }

            var parentMissionName = worksheet.Name;
            using var savingMemoryStream = new MemoryStream();
            workBook.SaveAs(savingMemoryStream);

            var fileDto = new MyFileInfoDto
            { FileContent = savingMemoryStream.ToArray(), FileName = parentMissionName };

            await SendEmail("每周任務報告", "這是你一周以來完成和過期的任務統計", emailMap[key.Value], fileDto);
        }
    }

    /// <summary>
    /// 檢查到期的任務
    /// </summary>
    [AllowAnonymous]
    public async Task CheckExpiredOrFinished()
    {
        // 檢查到期任務，若到期將IsActive設為false
        var now = Clock.Now;
        var missions = await _repositoys.Mission.GetListAsync(m =>
            m.MissionEndTime.CompareTo(now) < 0 && m.MissionFinishTime == null);
        missions.ForEach(m => m.IsActive = false);
    }

    #region 任務附件

    /// <summary>
    /// 上傳任務附件
    /// </summary>
    public async Task<FileInfoDto> UploadFile(Guid? teamId, Guid missionId, string name, int fileIndex, string note, IFormFile file)
    {
        return await _fileAppService.UploadAttachment(teamId, missionId, fileIndex, name, note, file);
    }

    /// <summary>
    /// 刪除任務附件
    /// </summary>
    /// <param name="id">附件 Id</param>
    public async Task DeleteFile(Guid id)
    {
        await _fileAppService.DeleteAttachment(id);
    }

    /// <summary>
    /// 取得某一任務所有附件
    /// </summary>
    /// <param name="id">任務 Id</param>
    public async Task<List<FileInfoDto>> GetAllFiles(Guid id)
    {
        return await _fileAppService.GetAllFiles(id);
    }

    /// <summary>
    /// 更新附件的備註
    /// </summary>
    /// <param name="id">附件 Id</param>
    public async Task UpdateAttachmentNote(Guid id, string note)
    {
       await _fileAppService.UpdateNote(id, note);
    }

    /// <summary>
    /// 下載附件
    /// </summary>
    /// <param name="id">附件 Id</param>
    public Task<MyFileInfoDto> DownloadFile(Guid id)
    {
        throw new NotImplementedException();
        //return new MyFileInfoDto
        //{ FileContent = savingMemoryStream.ToArray(), FileName = $"{parent.MissionName}的匯出檔案.xlsx" };
    }

    #endregion 任務附件

    /// <summary>
    /// 設定自己的定時任務
    /// </summary>
    private async Task CreateTaskSchedule(CreateOrUpdateMissionDto input)
    {
        // TODO 改成insert重複的資料到DB
        // 修改定時狀態
        input.Id = null;
        // 取消定時任務(刪除)
        var query = await _repositoys.MissionView.GetQueryableAsync();
        var idLangMaps = query.Where(m =>
                m.ScheduleMissionId == input.ScheduleMissionId && m.MissionId != input.ScheduleMissionId)
            .GroupBy(m => new { m.MissionId, m.Lang }).ToDictionary(m => m.Key, m => m.First().Lang);
        foreach (var idLangMap in idLangMaps)
        {
            await DeleteData(idLangMap.Key.MissionId, idLangMap.Value);
        }

        // 新增定時任務(新增)
        if (input.Schedule != 0)
        {
            var currentUserId = CurrentUser.Id;
            Mission baseMission = ObjectMapper.Map<CreateOrUpdateMissionDto, Mission>(input);
            for (int i = 0; i < maxScheduleCount; i++)
            {
                // 定時天數
                var days = input.Schedule == 1 ? 1 : input.Schedule == 2 ? 7 : 30;
                var mission = ObjectMapper.Map<CreateOrUpdateMissionDto, Mission>(input);
                var missionI18N = ObjectMapper.Map<CreateOrUpdateMissionDto, MissionI18N>(input);
                mission.MissionStartTime = baseMission.MissionStartTime.AddDays(days);
                mission.MissionEndTime = baseMission.MissionEndTime.AddDays(days);
                mission.UserId = currentUserId;
                mission.MissionI18Ns = new List<MissionI18N>();
                mission.MissionI18Ns.Add(missionI18N);
                baseMission = mission;
                await _repositoys.Mission.InsertAsync(mission, autoSave: true);
            }
        }
    }

    /// <summary>
    /// 設定excel下拉選單選項
    /// </summary>
    private void SetExcelFormattion(ref IXLWorksheet worksheet, List<string> options, char column)
    {
        var dropListRange = worksheet.Range($"{column}{ExcelBeginLine}:{column}{ExcelEndLine}");
        var dropValidation = dropListRange.SetDataValidation();
        var optionString = $"\"{String.Join(",", options)}\"";
        dropValidation.List(optionString, true);
    }

    private async Task DeleteData(Guid id, int lang)
    {
        // 透過missionId和lang共同判斷要刪除的I18N
        await _repositoys.MissionI18N.DeleteAsync(x => x.MissionId == id
                                                       && x.Lang == lang, autoSave: true);
        // 若該任務不存在任何語系則刪除任務本體
        var count = await _repositoys.MissionI18N.CountAsync(x => x.MissionId == id);
        if (count == 0)
        {
            await _repositoys.Mission.DeleteAsync(id);
        }
    }
}