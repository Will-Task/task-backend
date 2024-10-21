using Business.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Business.OrganizationManagement;

[Authorize(BusinessPermissions.TaskOrganization.Default)]
[RemoteService(false)]
public class OrganizationAppService : ApplicationService , IOrganizationAppService
{
    
}