using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Business.Enums;
using Business.FileManagement;
using Business.FileManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;
using Business.Permissions;
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
using XCZ.Extensions;

namespace Business.MissionManagement;

[Authorize(BusinessPermissions.TaskItem.Default)]
[RemoteService(false)]
public class MissionAppService : ApplicationService, IMissionAppService
{
    private (
        IRepository<Mission, Guid> Mission,
        IRepository<MissionI18N, Guid> MissionI18N,
        IRepository<MissionView> MissionView,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18N,
        IRepository<MissionCategoryView> MissionCategoryView
        ) _repositoys;

    private readonly IFileAppService _fileAppService;
    private readonly IConfiguration _Configuration;
    private readonly ILogger<MissionAppService> _logger;
    private readonly IDataFilter _dataFilter;
    private readonly int ExcelBeginLine = 4;

    private readonly int ExcelEndLine = 13;

    // 設定定時任務最大數量
    private readonly int maxScheduleCount = 10;

    // 根據MissionImportDto除去匯入檢查不需要的欄位
    private readonly List<string> ImportNotIncluded = new List<string>
    {
        "TeamId", "UserId", "ParentMissionId", "Lang" , "Id"
    };

    public MissionAppService(IRepository<Mission, Guid> Mission,
        IRepository<MissionI18N, Guid> MissionI18N,
        IRepository<MissionView> MissionView,
        IRepository<MissionCategoryI18N, Guid> MissionCategoryI18N,
        IRepository<MissionCategoryView> MissionCategoryView,
        IFileAppService fileAppService,
        IConfiguration Configuration,
        IDataFilter dataFilter,
        ILogger<MissionAppService> logger)
    {
        _repositoys = (Mission, MissionI18N, MissionView, MissionCategoryI18N,
            MissionCategoryView);
        _fileAppService = fileAppService;
        _Configuration = Configuration;
        _dataFilter = dataFilter;
        _logger = logger;
    }

    #region CRUD方法

    #endregion

    /// <summary>
    /// 新增/修改任務
    /// </summary>
    public async Task<MissionI18NDto> DataPost(CreateOrUpdateMissionDto input)
    {
        // 建立新missionI18N，供新增或新增語系
        var newMissionI18N = new MissionI18N
        {
            MissionName = input.MissionName,
            MissionDescription = input.MissionDescription,
            Lang = input.Lang,
        };
        if (input.Id.HasValue)
        {
            newMissionI18N.MissionId = input.Id.Value;
        }

        // 1. 判斷修改(對現有任務增加別的語系)或新增
        // 修改
        if (input.Id.HasValue)
        {
            var mission = await _repositoys.Mission.GetAsync(input.Id.Value);
            // 加載關聯I18N資訊
            await _repositoys.Mission.EnsureCollectionLoadedAsync(mission, m => m.MissionI18Ns);
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

                // 更新本體
                ObjectMapper.Map(input, mission);
                // 更新I18N info
                missionI18N.MissionId = input.Id.Value;
                missionI18N.MissionName = input.MissionName;
                missionI18N.MissionDescription = input.MissionDescription;
                missionI18N.Lang = input.Lang;
            }
            // 不存在 => 增加現有任務的語系
            else
            {
                missionI18Ns.Add(newMissionI18N);
            }

            await _repositoys.Mission.UpdateAsync(mission);
        }
        // 新增任務(不管任何語系，該任務皆不存在)
        else
        {
            // 儲存任務和I18N
            input.Id = GuidGenerator.Create();
            var newMission = ObjectMapper.Map<CreateOrUpdateMissionDto, Mission>(input);
            // 新建任務為TODO
            newMission.MissionState = input.MissionState;
            newMission.UserId = CurrentUser.Id;
            newMission.Email = CurrentUser.Email;
            newMission.ScheduleMissionId = input.Id;
            newMission.TeamId = input.TeamId;
            newMission.MissionI18Ns = new List<MissionI18N>();
            newMission.MissionI18Ns.Add(newMissionI18N);

            // 定時任務新增
            if (input.Schedule.HasValue)
            {
                input.ScheduleMissionId = newMission.Id;
                await CreateTaskSchedule(input);
            }

            // 儲存任務I18N(newMission就會有值且此時Guid是有優化過順具的)
            await _repositoys.Mission.InsertAsync(newMission, autoSave: true);
        }

        return ObjectMapper.Map<CreateOrUpdateMissionDto, MissionI18NDto>(input);
    }

    /// <summary>
    /// 刪除任務(單 or 多筆)
    /// </summary>
    public async Task Delete(Guid id, int lang)
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

    /// <summary>
    /// 獲取父任務下的子任務(多個)
    /// </summary>
    public async Task<PagedResultDto<MissionViewDto>> GetSubMission(Guid id, int page, int pageSize, bool allData)
    {
        var query = await _repositoys.MissionView.GetQueryableAsync();
        query = query.Where(mv => mv.ParentMissionId == id);
        var totalCount = await query.CountAsync();
        var subMissions = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var dtos = ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(subMissions);
        return new PagedResultDto<MissionViewDto>(totalCount, dtos);
    }

    /// <summary>
    /// 查詢所有父任務(多個)
    /// </summary>
    public async Task<PagedResultDto<MissionViewDto>> GetParentMission(int page, int pageSize, bool allData,
        Guid? teamId)
    {
        // 1. 抓該當前使用者mission & 所有parentId為null的(直接透過sql中的view抓)
        var currentUserId = CurrentUser.Id;
        var query = await _repositoys.MissionView.GetQueryableAsync();
        query = query.Where(mv => mv.ParentMissionId == null && mv.UserId == currentUserId && mv.TeamId == teamId);
        var count = await query.CountAsync();
        // 拿全部or分頁
        var parents = allData
            ? await query.ToListAsync()
            : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        var dtos = ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(parents);
        dtos = allData ? dtos : dtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return new PagedResultDto<MissionViewDto>(count, dtos);
    }

    /// <summary>
    /// 根據類別取得父任務
    /// </summary>
    public async Task<IEnumerable<MissionViewDto>> GetParentMissionByCategoryId(Guid categoryId)
    {
        // 根據類別取得父任務(parentId = null)
        var missionViews = await _repositoys.MissionView.GetListAsync(mv =>
            mv.ParentMissionId == null && mv.MissionCategoryId == categoryId);
        return ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(missionViews);
    }

    /// <summary>
    /// 查詢特定任務(單個)
    /// </summary>
    public async Task<MissionViewDto> Get(Guid id)
    {
        // 1. 撈特定類別的任務(直接透過sql中的view抓)
        var mission = await _repositoys.MissionView.GetAsync(
            mv => mv.MissionId == id);
        return ObjectMapper.Map<MissionView, MissionViewDto>(mission);
    }

    /// <summary>
    /// 查詢特定類別下的任務(多個)
    /// </summary>
    public async Task<IEnumerable<MissionViewDto>> GetMission(Guid id)
    {
        // 1. 撈特定類別的任務(直接透過sql中的view抓)
        var missionViews = await _repositoys.MissionView.GetListAsync(
            mv => mv.MissionCategoryId == id);
        return ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(missionViews);
    }

    /// <summary>
    /// 任務提醒通知(寄email)
    /// </summary>
    [AllowAnonymous]
    public async Task MissionReminder()
    {
        var missions = await _repositoys.Mission.GetListAsync();

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
            await SendEmail("任務提醒通知", "你的任務快到期了喔，趕緊完成!", mission.Email, null);
        }
    }

    /// <summary>
    /// 設置任務提醒時間(結束時間多久前)
    /// </summary>
    public async Task setRemindTime(Guid id, int hour)
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
    public async Task<MyFileInfoDto> DNSample(string fileName, int lang, Guid teamId)
    {
        var currentUserId = CurrentUser.Id;
        // 根據名稱取得範本
        var myFileInfo = await _fileAppService.DNFile(fileName);

        using var memoryStream = new MemoryStream(myFileInfo.FileContent);
        using var workbook = new XLWorkbook(memoryStream);
        var worksheet = workbook.Worksheet(1);
        worksheet.Name = myFileInfo.FileName;

        // 父類別任務名稱下拉選單設定
        var parentMissions = await _repositoys.MissionView.GetQueryableAsync();
        var parentNames = parentMissions
            .Where(pn => pn.UserId == currentUserId && pn.ParentMissionId == null && pn.Lang == lang && pn.TeamId == teamId)
            .Select(p => p.MissionName).ToList();
        SetExcelFormattion(ref worksheet, parentNames, 'A');

        // 類別下拉選單設定
        var categories = await _repositoys.MissionCategoryView.GetQueryableAsync();
        var categoryNames = categories.Where(c => c.UserId == currentUserId && c.Lang == lang  && c.TeamId == teamId)
            .Select(mcn => mcn.MissionCategoryName).ToList();
        SetExcelFormattion(ref worksheet, categoryNames, 'B');

        // 任務優先度下拉選單設定(1~5)
        var priority = new List<string> { "1", "2", "3", "4", "5" };
        SetExcelFormattion(ref worksheet, priority, 'J');

        // TODO
        // 任務狀態下拉選單設定
        
        // TODO
        // 任務提醒時間下拉選單設定

        // 日期格式設定
        var rangeDate = worksheet.Range($"E{ExcelBeginLine}:F{ExcelEndLine}");
        foreach (var cell in rangeDate.Cells())
        {
            cell.Style.NumberFormat.Format = "yyyy/m/d h:mm:ss";
        }

        // 設置固定區塊為唯讀
        var range = worksheet.Range($"A1:F{ExcelEndLine}");
        range.Style.Protection.Locked = true;

        using var savingMemoryStream = new MemoryStream();
        workbook.SaveAs(savingMemoryStream);
        myFileInfo.FileContent = savingMemoryStream.ToArray();

        return myFileInfo;
    }

    /// <summary>
    /// 資料匯入檢查
    /// </summary>
    public async Task<IEnumerable<MissionImportDto>> ImportFileCheck(IFormFile file, int lang, Guid? teamId)
    {
        try
        {
            var currentUserId = CurrentUser.Id;
            var dtos = new List<MissionImportDto>();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            using var workbook = new XLWorkbook(memoryStream);
            var worksheet = workbook.Worksheet(1);

            // 取得所有當前用者、所在團隊 的所有 name mapping Id
            var queryCategory = await _repositoys.MissionCategoryView.GetQueryableAsync();
            var categoryMap = queryCategory
                .Where(x => x.Lang == lang && x.UserId == currentUserId && x.TeamId == teamId)
                .ToDictionary(x => x.MissionCategoryName, x => x.MissionCategoryId);

            var queryMission = await _repositoys.MissionView.GetQueryableAsync();
            var missionMap = queryMission
                .Where(x => x.Lang == lang && x.UserId == currentUserId && x.TeamId == teamId)
                .ToDictionary(x => x.MissionName, x => x.MissionId);

            // 從excel取出資料
            for (int i = ExcelBeginLine; i <= ExcelEndLine; i++)
            {
                // 若種類沒被選擇，代表該row沒被填 -> 直接結束
                if (worksheet.Cell($"B{i}").IsEmpty())
                {
                    break;
                }

                var dto = new MissionImportDto();
                dto.UserId = currentUserId;
                dto.TeamId = teamId;
                dto.Lang = lang;

                var nextchar = 'A';
                // 根據excelKey對物件賦值
                foreach (var propertyInfo in dto.GetType().GetProperties()
                             .Where(x => !ImportNotIncluded.Contains(x.Name)))
                {
                    var value = $"{worksheet.Cell($"{nextchar}{i}").Value}";
                    if (propertyInfo.PropertyType == typeof(Guid))
                    {
                        propertyInfo.SetValue(dto, categoryMap[value]);
                    }
                    else if (propertyInfo.PropertyType == typeof(int) ||
                             propertyInfo.PropertyType == typeof(MissionState))
                    {
                        propertyInfo.SetValue(dto, Convert.ToInt32(value));
                    }
                    else if (propertyInfo.PropertyType == typeof(DateTime))
                    {
                        if (value.IsNullOrEmpty())
                        {
                            continue;
                        }

                        string format = "yyyy/M/d tt h:mm:ss";
                        var culture = new CultureInfo("zh-TW");
                        DateTime.TryParseExact(value, format, culture,
                            DateTimeStyles.AllowWhiteSpaces, out DateTime dateTime);
                        propertyInfo.SetValue(dto, dateTime);
                    }
                    else if(propertyInfo.PropertyType == typeof(string))
                    {
                        if (!value.IsNullOrEmpty())
                        {
                            propertyInfo.SetValue(dto, value);
                        }

                        if (propertyInfo.Name == "MissionName")
                        {
                            dto.ParentMissionId = missionMap[value];
                        }
                    }

                    nextchar++;
                }

                dtos.Add(dto);
            }

            return dtos;
        }
        catch (Exception e)
        {
            _logger.LogInformation(
                $"===========================mission import error {e.StackTrace.ToString()}================================================");
            throw new BusinessException("500", "import file check Wrong !!!!!!");
        }
    }

    /// <summary>
    /// 資料匯入
    /// </summary>
    public async Task ImportFile(List<MissionImportDto> dtos)
    {
        var missions = new List<Mission>();
        var currentUserId = CurrentUser.Id;

        // 將讀出資料進行匯入
        foreach (var dto in dtos)
        {
            var mission = ObjectMapper.Map<MissionImportDto, Mission>(dto);
            dto.MissionName = dto.MissionName.IsNullOrEmpty()
                ? dto.SubMissionName
                : dto.MissionName;
            mission.UserId = currentUserId;
            mission.Email = CurrentUser.Email;
            mission.MissionI18Ns = new List<MissionI18N>();
            mission.MissionI18Ns.Add(new MissionI18N
            {
                MissionName = dto.SubMissionName,
                MissionDescription = dto.SubMissionDescription,
                Lang = dto.Lang
            });
            missions.Add(mission);
            await _repositoys.Mission.InsertAsync(mission, autoSave: true);
        }
    }

    /// <summary>
    /// 資料匯出(子任務)
    /// </summary>
    public async Task<MyFileInfoDto> ExportFile(List<Guid> parentIds, int lang)
    {
        // 增加欄位，為了匯出資料
        var columns = new List<string>() { "任務名稱", "類別", "優先程度", "進度", "開始時間", "截止時間", "完成時間", "任務描述" };
        var extraImportKeys = new List<string>() { "MissionFinishTime", "MissionDescription" };
        extraImportKeys = ExcelKeys.Concat(extraImportKeys).ToList();

        // 取得欲寫入的範本檔案
        var fileInfoDto = await _fileAppService.DNFile("匯出子任務.xlsx");
        using var memoryStream = new MemoryStream(fileInfoDto.FileContent);
        using var workBook = new XLWorkbook(memoryStream);

        // 開始的工作表
        int startWorkSheet = 2;
        int columnLine = 1;

        foreach (var parentId in parentIds)
        {
            // 取得父任務
            var parentMission = await _repositoys.MissionView.FirstAsync(mv =>
                mv.MissionId == parentId && mv.Lang == lang);
            // 取得parentId的子任務
            var subMissions = await _repositoys.MissionView.GetListAsync(mv =>
                mv.ParentMissionId == parentId && mv.Lang == lang);
            int start = 2;
            var nextChar = 'A';

            IXLWorksheet worksheet;
            int index = 0;
            while (workBook.TryGetWorksheet(parentMission.MissionName, out var name))
            {
                index++;
            }

            if (!workBook.TryGetWorksheet("工作表1", out worksheet))
            {
                // 指定workSheet名字為當前匯出的父任務(動態生成tab)
                worksheet = workBook.AddWorksheet(
                    index == 0 ? parentMission.MissionName : parentMission.MissionName + $"({index})",
                    startWorkSheet++);
            }
            else
            {
                worksheet.Name = parentMission.MissionName;
            }

            // Column Name寫入
            for (int i = 0; i < columns.Count; i++)
            {
                worksheet.Cell($"{nextChar++}{columnLine}").Value = columns[i];
            }

            // 資料寫入檔案
            foreach (var subMission in subMissions)
            {
                nextChar = 'A';
                for (int i = 0; i < extraImportKeys.Count; i++)
                {
                    // 子任務
                    PropertyInfo propertyInfo = subMission.GetType().GetProperty(extraImportKeys[i]);
                    worksheet.Cell($"{nextChar++}{start}").Value = $"{propertyInfo.GetValue(subMission)}";
                }

                start++;
            }
        }

        using var savingMemoryStream = new MemoryStream();
        workBook.SaveAs(savingMemoryStream);

        return new MyFileInfoDto { FileContent = savingMemoryStream.ToArray(), FileName = "111" };
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
                mission.Value.Email);
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
        var fileInfoDto = await _fileAppService.DNFile("每周輸出報告.xlsx");
        using var memoryStream = new MemoryStream(fileInfoDto.FileContent);
        using var workBook = new XLWorkbook(memoryStream);
        var worksheet = workBook.Worksheet(1);

        // 根據不同子任務寫不同tab中，tab以父任務名稱為主
        var query = await _repositoys.MissionView.GetQueryableAsync();
        // key : userId , value : subMissions
        var submissions = query.Where(m => m.ParentMissionId != null && m.UserId != null)
            .GroupBy(m => m.UserId)
            .ToDictionary(m => m.Key, m => m.ToList());

        foreach (var key in submissions.Keys)
        {
            int i = ExcelBeginLine;
            string email = "";

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

                email = submission.Email;
                i++;
            }

            var parentMissionName = worksheet.Name;
            using var savingMemoryStream = new MemoryStream();
            workBook.SaveAs(savingMemoryStream);

            var fileDto = new MyFileInfoDto
            { FileContent = savingMemoryStream.ToArray(), FileName = parentMissionName };

            await SendEmail("每周任務報告", "這是你一周以來完成和過期的任務統計", email, fileDto);
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

    /// <summary>
    /// 上傳任務附件
    /// </summary>
    public async Task<MissionAttachmentDto> UploadFile(CreateMissionAttachmentDto input, IFormFile file)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 刪除任務附件
    /// </summary>
    /// <param name="id">附件 Id</param>
    public async Task DeleteFile(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 取得某一任務所有附件
    /// </summary>
    /// <param name="id">任務 Id</param>
    public async Task<List<MissionAttachmentDto>> GetAllFiles(Guid id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 更新附件的備註
    /// </summary>
    /// <param name="id">附件 Id</param>
    public async Task UpdateAttachmentNote(Guid id)
    {
        throw new NotImplementedException();
    }

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
                mission.Email = CurrentUser.Email;
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