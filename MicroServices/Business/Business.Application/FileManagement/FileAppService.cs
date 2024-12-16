using Business.Enums;
using Business.FileManagement.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Business.Models;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using FileInfo = Business.Models.FileInfo;
using Microsoft.Extensions.Logging;

namespace Business.FileManagement;

public class FileAppService : ApplicationService, IFileAppService
{
    private readonly IRepository<FileInfo, Guid> _repository;
    private readonly FileManager _fileManager;
    private readonly ILogger<FileAppService> _logger;

    List<string> pictureFormatArray = new List<string>
        { ".png", ".jpg", ".jpeg", ".gif", ".PNG", ".JPG", ".JPEG", ".GIF" };

    public FileAppService(IRepository<FileInfo, Guid> repository, FileManager fileManager, ILogger<FileAppService> logger)
    {
        _repository = repository;
        _fileManager = fileManager;
        _logger = logger;
    }

    /// <summary>
    /// 檔案匯入
    /// </summary>
    public async Task<FileInfoDto> Upload([Required] string name, [Required] IFormFile file)
    {
        if (file == null || file.Length == 0) throw new BusinessException("無法上傳空文件");

        //限制100M
        if (file.Length > 104857600)
        {
            throw new BusinessException("上傳文件過大");
        }

        //文件格式
        var fileExtension = Path.GetExtension(file.FileName);
        if (!pictureFormatArray.Contains(fileExtension))
        {
            throw new BusinessException("上傳文件格式錯誤");
        }

        var size = "";
        if (file.Length < 1024)
            size = file.Length.ToString() + "B";
        else if (file.Length >= 1024 && file.Length < 1048576)
            size = ((float)file.Length / 1024).ToString("F2") + "KB";
        else if (file.Length >= 1048576 && file.Length < 104857600)
            size = ((float)file.Length / 1024 / 1024).ToString("F2") + "MB";
        else size = file.Length.ToString() + "B";

        string uploadsFolder = Path.Combine(Environment.CurrentDirectory, "wwwroot", "samples");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = name;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(fileStream);
            fileStream.Flush();
        }

        //TODO：文件md5哈希校驗
        var result = await _fileManager.Create(name, uniqueFileName, fileExtension, "", size, filePath,
            "/samples/" + uniqueFileName, FileType.IMAGE);
        return ObjectMapper.Map<FileInfo, FileInfoDto>(result);
    }

    /// <summary>
    /// 取得範本檔案
    /// </summary>
    public async Task<BlobDto> DNFile(string fileName)
    {
        // 透過名稱撈檔案路徑
        var filePath = Path.Combine(Environment.CurrentDirectory, "wwwroot", "templates", fileName);
        if (!File.Exists(filePath)) throw new BusinessException("找不到文件");
        var bytes = await File.ReadAllBytesAsync(filePath);
        return new BlobDto { Name = fileName, Content = bytes };
    }

    #region 附件相關操作

    /// <summary>
    /// 上傳附件
    /// </summary>
    public async Task<FileInfoDto> UploadAttachment(Guid? teamId, Guid missionId, int fileIndex, string name, string note,
        IFormFile file)
    {
        if (file == null || file.Length == 0) throw new BusinessException("無法上傳空文件");

        //限制100M
        if (file.Length > 104857600)
        {
            throw new BusinessException("上傳文件過大");
        }

        //文件格式
        var fileExtension = Path.GetExtension(file.FileName);
        if (!pictureFormatArray.Contains(fileExtension))
        {
            throw new BusinessException("上傳文件格式錯誤");
        }

        var size = "";
        if (file.Length < 1024)
            size = file.Length.ToString() + "B";
        else if (file.Length >= 1024 && file.Length < 1048576)
            size = ((float)file.Length / 1024).ToString("F2") + "KB";
        else if (file.Length >= 1048576 && file.Length < 104857600)
            size = ((float)file.Length / 1024 / 1024).ToString("F2") + "MB";
        else size = file.Length.ToString() + "B";

        string uploadsFolder = Path.Combine(Environment.CurrentDirectory, "wwwroot", "attachment");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(fileStream);
            fileStream.Flush();
        }

        //TODO：文件md5哈希校驗
        var result = await _fileManager.Create(CurrentUser.Id, teamId, missionId, note, fileIndex, name, uniqueFileName, fileExtension, "", size, filePath,
            "/attachment/" + uniqueFileName, FileType.IMAGE);
        return ObjectMapper.Map<FileInfo, FileInfoDto>(result);
    }

    /// <summary>
    /// 獲取某任務下所有附件
    /// </summary>
    /// <param name="id"> 任務Id </param>
    public async Task<List<FileInfoDto>> GetAllFiles(Guid id)
    {
        var attachments = await _repository.GetListAsync(x => x.MissionId == id);
        return ObjectMapper.Map<List<FileInfo>, List<FileInfoDto>>(attachments);
    }

    /// <summary>
    /// 更新備註
    /// </summary>
    /// <param name="id"> 任務Id </param>
    public async Task UpdateNote(Guid id, string note)
    {
        var attachment = await _repository.GetAsync(id);
        attachment.Note = note;
    }

    /// <summary>
    /// 更新備註
    /// </summary>
    /// <param name="id"> 附件Id </param>
    public async Task DeleteAttachment(Guid id)
    {
        var attachment = await _repository.GetAsync(id);
        // 刪除路徑中對應的照片
        var path = Environment.CurrentDirectory + "/wwwroot" + attachment.Url;
        _logger.LogError($"=============================== 照片路徑 {path}");

        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            throw new BusinessException("照片不存在");
        }
        await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// 獲取某特定任務的附件數量
    /// </summary>
    /// <param name="id"> 任務id </param>
    public async Task<int> GetAttachmentCount(Guid id)
    {
        return await _repository.CountAsync(x => x.MissionId == id);
    }

    #endregion 附件相關操作
}