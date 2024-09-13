using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using Business.Enums;
using Business.FileManagement;
using Business.FileManagement.Dto;
using Business.MissionManagement.Dto;
using Business.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Business.MissionManagement;

[Authorize]
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

    /// <summary>
    /// 獲取父任務下的子任務(多個)
    /// </summary>
    public async Task<IEnumerable<MissionViewDto>> GetSubMission(Guid id)
    {
        // 抓特定富任務下的子任務(直接透過sql中的view抓)
        var subMissions = await _repositoys.MissionView.GetListAsync(mv => mv.ParentMissionId == id);
        return ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(subMissions);
    }

    /// <summary>
    /// 查詢所有父任務(多個)
    /// </summary>
    public async Task<IEnumerable<MissionViewDto>> GetParentMission()
    {
        // 1. 抓該當前使用者mission & 所有parentId為null的(直接透過sql中的view抓)
        var currentUserId = CurrentUser.Id;
        var parentMissions = await _repositoys.MissionView.GetListAsync(
            mv => mv.ParentMissionId == null && mv.UserId == currentUserId);
        return ObjectMapper.Map<List<MissionView>, List<MissionViewDto>>(parentMissions);
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

        foreach (var mission in missions)
        {
            var now = Clock.Now;
            now = now.AddHours(mission.MissionBeforeEnd);
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
    /// 新增/修改任務
    /// </summary>
    public async Task<MissionI18NDto> DataPost(CreateOrUpdateMissionDto input)
    {
        // 判斷開始時間是否有早於結束時間
        if (input.MissionStartTime.CompareTo(input.MissionEndTime) == 1)
        {
            throw new UserFriendlyException("結束時間比開始時間還晚", "404");
        }

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
                // 更新I18N
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
            var newMission = ObjectMapper.Map<CreateOrUpdateMissionDto, Mission>(input);
            // 新建任務為TODO
            newMission.MissionState = MissionState.TO_DO;
            newMission.UserId = CurrentUser.Id;
            newMission.Email = CurrentUser.Email;
            newMission.MissionI18Ns = new List<MissionI18N>();
            newMission.MissionI18Ns.Add(newMissionI18N);

            // 儲存任務I18N(newMission就會有值且此時Guid是有優化過順具的)
            await _repositoys.Mission.InsertAsync(newMission, autoSave: true);
            // 指定剛剛新增的任務ID
            input.Id = newMission.Id;
        }

        return ObjectMapper.Map<CreateOrUpdateMissionDto, MissionI18NDto>(input);
    }

    /// <summary>
    /// 變更任務狀態
    /// </summary>
    public async Task UpdateMissionState(Guid missionId, int state)
    {
        var mission = await _repositoys.Mission.GetAsync(missionId);
        mission.MissionState = (MissionState)state;
    }

    /// <summary>
    /// 刪除任務(過期任務不會被刪除)
    /// </summary>
    /// <param name="id">任務I18N的ID。</param>
    public async Task Delete(Guid id, int lang)
    {
        // 透過missionId和lang共同判斷要刪除的I18N
        await _repositoys.MissionI18N.DeleteAsync(x => x.MissionId == id
                                                   && x.Lang == lang, autoSave: true);

        // 若該任務不存在任何語系則刪除任務本體
        var num = await _repositoys.MissionI18N.CountAsync(x => x.MissionId == id);
        if (num == 0)
        {
            await _repositoys.Mission.DeleteAsync(id, autoSave: true);
        }
    }

    /// <summary>
    /// 刪除任務(過期任務不會被刪除，刪除某父任務下的子任務)
    /// </summary>
    public async Task DeleteGroup(List<Guid> subIds, Guid parentId)
    {
        await _repositoys.MissionI18N.DeleteManyAsync(subIds, autoSave: true);

        // 若沒有關聯I18N，刪除mission本體
        var num = await _repositoys.Mission.GetCountAsync();

        _logger.LogInformation(
            $"===============================mission還有關聯的I18N資料，共有{num}筆================================================");

        if (num == 0)
        {
            await _repositoys.Mission.DeleteAsync(parentId);
        }
    }

    private List<string> ExcelKeys = new List<string>
    {
        "MissionName", "MissionCategoryName", "MissionPriority",
        "MissionState", "MissionStartTime", "MissionEndTime"
    };

    /// <summary>
    /// 範本下載
    /// </summary>
    public async Task<MyFileInfoDto> DNSample(string fileName, int lang)
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
            .Where(pn => pn.UserId == currentUserId && pn.ParentMissionId == null && pn.Lang == lang)
            .Select(p => p.MissionName).ToList();
        SetExcelFormattion(ref worksheet, parentNames, 'A');

        // 類別下拉選單設定
        var categories = await _repositoys.MissionCategoryView.GetQueryableAsync();
        var categoryNames = categories.Where(c => c.UserId == currentUserId && c.Lang == lang)
            .Select(mcn => mcn.MissionCategoryName).ToList();
        SetExcelFormattion(ref worksheet, categoryNames, 'B');

        // 任務優先度下拉選單設定(1~5)
        var priority = new List<string> { "1", "2", "3", "4", "5" };
        SetExcelFormattion(ref worksheet, priority, 'C');

        // TODO
        // 任務狀態下拉選單設定

        // 日期格式設定
        var rangeDate = worksheet.Range($"E{ExcelBeginLine}:F{ExcelEndLine}");
        foreach (var cell in rangeDate.Cells())
        {
            cell.Style.NumberFormat.Format = "yyyy/m/d h:mm:ss";
        }

        // 語言設定
        var languages = new List<string> { "1", "2" };
        SetExcelFormattion(ref worksheet, languages, 'I');

        // 設置固定區塊為唯讀
        var range = worksheet.Range($"A1:F{ExcelEndLine}");
        range.Style.Protection.Locked = true;

        using var savingMemoryStream = new MemoryStream();
        workbook.SaveAs(savingMemoryStream);

        return new MyFileInfoDto { FileContent = savingMemoryStream.ToArray(), FileName = myFileInfo.FileName };
    }

    /// <summary>
    /// 資料匯入
    /// </summary>
    public async Task<IEnumerable<MissionImportDto>> ImportFile(IFormFile file, int lang)
    {
        try
        {
            var currentUserId = CurrentUser.Id;
            // 增加欄位，為了取得新增資料
            var extraKeys = new List<string> { "SubMissionName", "SubMissionDescription", "SubMissionLang" };
            extraKeys = ExcelKeys.Concat(extraKeys).ToList();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            using var workbook = new XLWorkbook(memoryStream);
            var worksheet = workbook.Worksheet(1);

            var importDtos = new List<MissionImportDto>();

            // 從excel取出資料
            for (int i = ExcelBeginLine; i <= ExcelEndLine; i++)
            {
                // 若種類沒被選擇，代表該row沒被填 -> 直接結束
                if (worksheet.Cell($"B{i}").IsEmpty())
                {
                    break;
                }

                var importDto = new MissionImportDto();
                var nextchar = 'A';
                // 根據excelKey對物件賦值
                foreach (var extraKey in extraKeys)
                {
                    var property = typeof(MissionImportDto).GetProperty(extraKey);
                    if (extraKey == "MissionCategoryName")
                    {
                        var categoryName = $"{worksheet.Cell($"{nextchar}{i}").Value}";
                        var query = await _repositoys.MissionCategoryI18N.GetQueryableAsync();
                        var categoryId = query.AsNoTracking().Where(mcn =>
                                mcn.MissionCategoryName == categoryName && mcn.Lang == lang)
                            .Select(mcn => mcn.MissionCategoryId).First();
                        property.SetValue(importDto, categoryName);
                        property = typeof(MissionImportDto).GetProperty("MissionCategoryId");
                        property.SetValue(importDto, categoryId);
                    }
                    else if (extraKey == "MissionName")
                    {
                        var parentMissionName = $"{worksheet.Cell($"{nextchar}{i}").Value}";
                        if (!parentMissionName.IsNullOrEmpty())
                        {
                            var query = await _repositoys.MissionView.GetQueryableAsync();
                            var parentMissionId = query.AsNoTracking().Where(mv =>
                                    mv.MissionName == parentMissionName && mv.Lang == lang)
                                .Select(mv => mv.ParentMissionId).First();
                            property.SetValue(importDto, parentMissionId.ToString());
                        }
                    }
                    else if (extraKey == "MissionPriority" || extraKey == "SubMissionLang")
                    {
                        property.SetValue(importDto, int.Parse($"{worksheet.Cell($"{nextchar}{i}").Value}"));
                    }
                    else if (extraKey == "MissionState")
                    {
                        property.SetValue(importDto,
                            (MissionState)int.Parse($"{worksheet.Cell($"{nextchar}{i}").Value}"));
                    }
                    else if (extraKey == "MissionStartTime" || extraKey == "MissionEndTime")
                    {
                        string format = "yyyy/M/d tt h:mm:ss";
                        var culture = new CultureInfo("zh-TW");
                        DateTime.TryParseExact($"{worksheet.Cell($"{nextchar}{i}").Value}", format, culture,
                            DateTimeStyles.AllowWhiteSpaces, out DateTime dateTime);
                        property.SetValue(importDto, dateTime);
                    }
                    else
                    {
                        property.SetValue(importDto, $"{worksheet.Cell($"{nextchar}{i}").Value}");
                    }

                    nextchar++;
                }

                importDtos.Add(importDto);
            }

            var missions = new List<Mission>();

            // 將讀出資料進行匯入
            foreach (var importDto in importDtos)
            {
                var mission = ObjectMapper.Map<MissionImportDto, Mission>(importDto);
                importDto.MissionName = importDto.MissionName.IsNullOrEmpty()
                    ? importDto.SubMissionName
                    : importDto.MissionName;
                mission.UserId = currentUserId;
                mission.MissionI18Ns = new List<MissionI18N>();
                mission.MissionI18Ns.Add(new MissionI18N
                {
                    MissionName = importDto.SubMissionName,
                    MissionDescription = importDto.SubMissionDescription,
                    Lang = importDto.SubMissionLang
                });
                missions.Add(mission);
            }

            await _repositoys.Mission.InsertManyAsync(missions);
            return importDtos;
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"===========================mission import error {e.StackTrace.ToString()}================================================");
            return null;
        }
    }

    /// <summary>
    /// 資料匯出(子任務)
    /// </summary>
    public async Task<MyFileInfoDto> ExportFile(Guid parentId, int lang)
    {
        // 增加欄位，為了匯出資料
        var extraKeys = new List<string> { "MissionName", "MissionDescription" };
        extraKeys = ExcelKeys.Concat(extraKeys).ToList();

        // 取得父任務
        var parentMission = await _repositoys.MissionView.FirstAsync(mv =>
            mv.MissionId == parentId && mv.Lang == lang);
        // 取得parentId的子任務
        var subMissions = await _repositoys.MissionView.GetListAsync(mv =>
            mv.ParentMissionId == parentId && mv.Lang == lang);

        // 取得欲寫入的範本檔案
        var fileInfoDto = await _fileAppService.DNFile("匯入子任務");
        using var memoryStream = new MemoryStream(fileInfoDto.FileContent);
        using var workBook = new XLWorkbook(memoryStream);
        var worksheet = workBook.Worksheet(1);
        // 指定workSheet名字為當前匯出的父任務
        worksheet.Name = parentMission.MissionName;
        int start = ExcelBeginLine;

        // 資料寫入檔案
        foreach (var subMission in subMissions)
        {
            var nextChar = 'A';
            for (int i = 0; i < extraKeys.Count; i++)
            {
                PropertyInfo propertyInfo;
                // 子任務
                if (i + 1 > ExcelKeys.Count)
                {
                    propertyInfo = subMission.GetType().GetProperty(extraKeys[i]);
                    worksheet.Cell($"{nextChar}{start}").Value = $"{propertyInfo.GetValue(subMission)}";
                }
                // 父任務
                else
                {
                    propertyInfo = parentMission.GetType().GetProperty(extraKeys[i]);
                    worksheet.Cell($"{nextChar}{start}").Value = $"{propertyInfo.GetValue(parentMission)}";
                }

                nextChar++;
            }

            start++;
        }

        using var savingMemoryStream = new MemoryStream();
        workBook.SaveAs(savingMemoryStream);

        return new MyFileInfoDto { FileContent = savingMemoryStream.ToArray(), FileName = parentMission.MissionName };
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
    /// 任務即將到期(24小時)通知
    /// </summary>
    [AllowAnonymous]
    public async Task IdentifyExpiringTasks()
    {
        var now = Clock.Now;
        // 避免讓條件過於複雜，使sql無法解析
        var oneDayLater = now.AddDays(1);
        // 結束時間扣掉當前時間小於1天
        var query = await _repositoys.Mission.GetQueryableAsync();
        var missionMap = query.Where(m => m.MissionEndTime <= oneDayLater && m.MissionFinishTime == null)
            .GroupBy(m => m.Id).ToDictionary(g => g.Key , g => g.First().Email);

        foreach (var mission in missionMap)
        {
            // 默認隨便拿一個寄
            var i18NQuery = await _repositoys.MissionI18N.GetQueryableAsync();
            var misssionName = i18NQuery.Where(mn => mn.MissionId == mission.Key)
                .Select(mn => mn.MissionName).FirstOrDefault();
            // 因為可能 Mission 中 isActive = false 的會找不到
            if (misssionName.IsNullOrEmpty())
            {
                continue;
            }

            // 寄發email(之後放入使用者email)
            await SendEmail(misssionName + "任務24小時內即將到期", "任務快過期了喔，趕緊去完成", mission.Value);
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
        var fileInfoDto = await _fileAppService.DNFile("每周輸出報告");
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

            var fileDto = new MyFileInfoDto { FileContent = savingMemoryStream.ToArray(), FileName = parentMissionName };
            
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
            m.MissionEndTime.CompareTo(now) == -1 && m.MissionFinishTime == null);
        missions.ForEach(m => m.IsActive = false);
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
}