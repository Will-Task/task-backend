using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto
{
    public class MissioGanttByOrderDto : EntityDto<Int32>
    {
        public Guid? PredecessorId { get; set; }

        public Guid? SuccessorId { get; set; }

        public int Type { get; set; } = 1;
    }
}
