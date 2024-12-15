using System;
using Business.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class FileInfo : AuditedAggregateRoot<Guid>, ISoftDelete
{
    public Guid? UserId { get; set; }
    
    public Guid? TeamId { get; set; }
    
    public Guid MissionId { get; set; }
    
    /// <summary>
    /// 代表該任務的第幾個附件
    /// </summary>
    public int FileIndex { get; set; }
    
    /// <summary>
    /// 任務附件備註說明
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 上傳時的檔名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 真實的檔名(Guid)
    /// </summary>
    public string RealName { get; set; }

    /// <summary>
    /// 后缀
    /// </summary>
    public string Suffix { get; set; }

    public string Md5Code { get; set; }

    public string Size { get; set; }

    public string Path { get; set; }

    public string Url { get; set; }

    public FileType Type { get; set; }

    public bool IsDeleted { get; set; }

    protected FileInfo()
    {
    }

    public FileInfo(Guid id, string name, string realName, string suffix, string md5code, string size, string path,
        string url, FileType type) : base(id)
    {
        Name = name;
        RealName = realName;
        Suffix = suffix;
        Md5Code = md5code;
        Size = size;
        Path = path;
        Url = url;
        Type = type;
    }
    
    public FileInfo(Guid id, Guid? userId, Guid? teamId, Guid missionId, string note, int fileIndex, string name, string realName, string suffix, string md5code, string size, string path,
        string url, FileType type) : base(id)
    {
        UserId = userId;
        TeamId = teamId;
        MissionId = missionId;
        Note = note;
        FileIndex = fileIndex;
        Name = name;
        RealName = realName;
        Suffix = suffix;
        Md5Code = md5code;
        Size = size;
        Path = path;
        Url = url;
        Type = type;
    }
}