using Business.AppsettingClass;
using Business.Localization;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Business
{
    [DependsOn(
        typeof(AbpLocalizationModule),
        typeof(AbpPermissionManagementApplicationContractsModule)
    )]
    public class BusinessApplicationContractsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<BusinessApplicationContractsModule>("Business");
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<BusinessResource>("zh-Hans")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/Business");
            });
            
            // using the options pattern is to bind the "TransientFaultHandlingOptions" section and add it to the dependency injection service container.
            Configure<EmailSettings>(configuration.GetSection(key: nameof(EmailSettings)));
        }
    }
}
