using System;
using Volo.Abp.Application.Dtos;

namespace Business.FileManagement.Dto;

public class MyFileInfoDto
{
    public string FileName { get; set; }
    
    public string FileType { get; set; }
    
    public string FilePath { get; set; }
    
    public byte[] FileContent { get; set; }
}