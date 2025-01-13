using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto
{
    public class MissioGanttByOrderDto : EntityDto<Guid>
    {
        public Guid? PredecessorID { get; set; }

        public Guid? SuccessorID { get; set; }

        public int Type { get; set; } = 1;
    }
}
