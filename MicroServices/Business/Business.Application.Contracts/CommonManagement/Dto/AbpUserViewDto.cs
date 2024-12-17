using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Business.CommonManagement.Dto
{
    public class AbpUserViewDto : EntityDto<Guid>
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
