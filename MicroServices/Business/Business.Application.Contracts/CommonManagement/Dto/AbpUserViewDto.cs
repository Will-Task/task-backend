using System;
using Volo.Abp.Application.Dtos;

namespace Business.CommonManagement.Dto
{
    public class AbpUserViewDto : EntityDto<Guid>
    {
        public string UserName { get; set; }

        public string Email { get; set; }
    }
}
