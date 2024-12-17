using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Business.Models
{
    public class AbpUserView : Entity<Guid>
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
