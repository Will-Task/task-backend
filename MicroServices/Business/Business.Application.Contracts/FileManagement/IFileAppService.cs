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
    /// 檔案匯入
    /// </summary>
    Task<FileInfoDto> Upload([Required] string name, [Required] IFormFile file);
    
    Task<PagedResultDto<FileInfoDto>> GetAll(GetFileInputDto input);

    Task UploadPrivate([Required] string name, [Required] IFormFile file);
}