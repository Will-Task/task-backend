using System;
using Volo.Abp.Domain.Entities;

namespace Business.Models;

public class Language : Entity<Int32>
{
    public string Code { get; set; }
    
    public string Name { get; set; }
}