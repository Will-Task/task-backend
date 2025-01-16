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

        public Guid? ParentId { get; set; }

        public int OrderId { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public decimal PercentComplete { get; set; }

        /// <summary>
        /// 是否為匯總任務
        /// </summary>
        public bool Summary { get; set; }

        /// <summary>
        /// 獨立任務要為 true
        /// </summary>
        public bool Expanded { get; set; }

        public int Lang { get; set; }
    }
}
