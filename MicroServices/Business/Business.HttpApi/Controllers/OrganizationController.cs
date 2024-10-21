using Business.OrganizationManagement;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace Business.Controllers;

[Area(("Organization"))]
[Route("api/business/org")]
public class OrganizationController : AbpController
{
    private readonly IOrganizationAppService _organizationAppService;

    public OrganizationController(IOrganizationAppService organizationAppService)
    {
        _organizationAppService = organizationAppService;
    }
}