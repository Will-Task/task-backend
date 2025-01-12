using System;
using System.Collections.Generic;

namespace Business.DashboardManagement.Dto;

public class CategoryPercentageDto
{
    public string CategoryName { get; set; }

    public List<int> Rates { get; set; }
    
    public int Lang { get; set; }
}