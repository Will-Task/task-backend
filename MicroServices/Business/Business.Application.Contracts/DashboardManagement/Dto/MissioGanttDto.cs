using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Business.DashboardManagement.Dto
{
    public class MissioGanttDto : EntityDto<Guid>
    {
        /// <summary>
        /// 任務名稱
        /// </summary>
        public string Title { get; set; }

        public Guid? ParentID { get; set; }

        public int OrderId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public decimal PercentComplete { get; set; }

        public bool Summary { get; set; } = true;

        public bool Expanded { get; set; } = true;

        public int Lang { get; set; }
    }
}
