using EasyAbp.NotificationService.Provider.PrivateMessaging;
using Volo.Abp.AuditLogging;
using Volo.Abp.Modularity;

namespace Business
{
    [DependsOn(
        typeof(AbpAuditLoggingDomainModule),
        //typeof(AbpSettingManagementDomainModule),
        typeof(NotificationServiceProviderPrivateMessagingModule)
    )]
    public class BusinessDomainModule : AbpModule
    {

    }
}
