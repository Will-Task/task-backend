using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Business.TeamManagement.Dto;

public class TeamDto : EntityDto<Guid?>
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }
    
    public int Year { get; set; }
}