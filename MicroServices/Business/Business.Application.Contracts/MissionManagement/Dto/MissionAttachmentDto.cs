using System;
using Business.Enums;
using Volo.Abp.Application.Dtos;

namespace Business.MissionManagement.Dto;

public class MissionAttachmentDto : EntityDto<Guid>
{
    public Guid MissionId { get; set; }
 
    public int FileIndex { get; set; }

    public string ANote { get; set; }

    public string Name { get; set; }

    public string RealName { get; set; }
    
    public string Suffix { get; set; }

    public string Md5Code { get; set; }

    public string Size { get; set; }

    public string Path { get; set; }

    public string Url { get; set; }

    /// <summary>
    /// 照片 1 , 檔案 2
    /// </summary>
    public FileType Type { get; set; }
}