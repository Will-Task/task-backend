using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Volo.Abp.Application.Services;

namespace Business.FileManagement;

public interface IFileAppService : IApplicationService
{
    /// <summary>
    /// 範文匯入
    /// </summary>
    Task Upload(ImportMyFileDto fileDto);

    /// <summary>
    /// 取得範本檔案
    /// </summary>
    Task<MyFileInfoDto> DNFile(string fileName);
}