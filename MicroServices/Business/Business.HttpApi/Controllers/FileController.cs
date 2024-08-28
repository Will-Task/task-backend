using System.Threading.Tasks;
using Business.FileManagement;
using Business.FileManagement.Dto;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area("file")]
[Route("/api/file")]
public class FileController : AbpController
{
    private readonly IFileAppService _fileAppService;

    public FileController(IFileAppService fileAppService)
    {
        _fileAppService = fileAppService;
    }

    /// <summary>
    /// 範文匯入
    /// </summary>
    [Route("upload")]
    [HttpPost]
    public Task upload(ImportMyFileDto importMyFileDto)
    {
        return _fileAppService.Upload(importMyFileDto);
    }
}