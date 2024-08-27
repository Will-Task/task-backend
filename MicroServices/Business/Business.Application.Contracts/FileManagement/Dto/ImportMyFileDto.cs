using System;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;

namespace Business.FileManagement.Dto;

public class ImportMyFileDto
{
    public string FileName { get; set; }
    
    public string FileType { get; set; }
    
    public IFormFile file { get; set; }
}