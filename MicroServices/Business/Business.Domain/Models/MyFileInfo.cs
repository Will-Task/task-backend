using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Business.Models;

public class MyFileInfo: AuditedAggregateRoot<Guid> , ISoftDelete
{
    public string FileName { get; set; }
    
    public string FileType { get; set; }
    
    public string FilePath { get; set; }
    
    public bool IsDeleted { get; set; }
}