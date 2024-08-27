using System;
using System.IO;
using System.Threading.Tasks;
using Business.FileManagement.Dto;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Business.FileManagement;

[Authorize]
public class FileAppService : ApplicationService, IFileAppService
{
    private readonly IRepository<MyFileInfo> _repository;

    public FileAppService(IRepository<MyFileInfo> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 範文匯入
    /// </summary>
    public async Task Upload(ImportMyFileDto fileInfoDto)
    {
        // 儲存範本到wwwroot下
        var file = fileInfoDto.file;
        var currentDirectory = Environment.CurrentDirectory;
        string fName = GuidGenerator.Create().ToString() + Path.GetExtension(file.FileName);
        string fileExtension = Path.GetExtension(file.Name);
        string filePath = Path.Combine(currentDirectory, "wwwroot", "samples", fName + fileExtension);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(fileStream);
            fileStream.Flush();
        }

        // 儲存到資料庫
        var myFileInfo = ObjectMapper.Map<ImportMyFileDto, MyFileInfo>(fileInfoDto);
        myFileInfo.FilePath = filePath;
        await _repository.InsertAsync(myFileInfo);

        // using var savingMemoryStream = new MemoryStream();
        // file.CopyTo(savingMemoryStream);
        //
        // using var workbook = new XLWorkbook(savingMemoryStream);
        // var worksheet = workbook.Worksheet(0);
        // worksheet.Name = fileName;
    }

    /// <summary>
    /// 取得範本檔案
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<MyFileInfoDto> DNFile(string fileName)
    {
        // 透過名稱撈檔案路徑
        var myFileInfo = await _repository.GetAsync(f => f.FileName == fileName);

        if (!File.Exists(myFileInfo.FilePath))
        {
            throw new UserFriendlyException("404", "根據範本路徑找不到檔案");
        }

        var bytes = await File.ReadAllBytesAsync(myFileInfo.FilePath);

        return new MyFileInfoDto
            { FileContent = bytes, FileName = myFileInfo.FileName, FilePath = myFileInfo.FilePath };
    }
}