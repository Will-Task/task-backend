using System.Collections.Generic;

namespace Business.DashboardManagement.Dto;

public class MissionOfEveryMonthDto
{
    public string CategoryName { get; set; }
    
    public List<int> FinishAmount { get; set; }
    
    public int Lang { get; set; }
}