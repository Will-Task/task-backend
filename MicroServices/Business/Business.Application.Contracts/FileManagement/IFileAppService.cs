using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Business.FileManagement;

public interface IFileAppService : IApplicationService
{
    /// <summary>
    /// 下載檔案
    /// </summary>
    Task<BlobDto> DNFile(string fileName);

    /// <summary>
    /// 獲取單一附件
    /// </summary>
    /// <param name="id"> 附件Id </param>
    Task<BlobDto> GetFile(Guid id);

    /// <summary>
    /// 獲取某任務下所有附件
    /// </summary>
    /// <param name="id"> 任務Id </param>
    Task<List<FileInfoDto>> GetAllFiles(Guid id);
    
    /// <summary>
    /// 檔案匯入
    /// </summary>
    Task<FileInfoDto> Upload([Required] string name, [Required] IFormFile file);
    
    /// <summary>
    /// 上傳附件
    /// </summary>
    Task<FileInfoDto> UploadAttachment(Guid? teamId, Guid missionId, int fileIndex, string name, string Note, IFormFile file);

    /// <summary>
    /// 更新備註
    /// </summary>
    /// <param name="id"> 任務Id </param>
    Task UpdateNote(Guid id, string note);
    
    /// <summary>
    /// 更新備註
    /// </summary>
    /// <param name="id"> 附件Id </param>
    Task DeleteAttachment(Guid id);

    /// <summary>
    /// 獲取某特定任務的附件數量
    /// </summary>
    /// <param name="id"> 任務id </param>
    Task<int> GetAttachmentCount(Guid id);
}