using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Business
{
    [DependsOn(
        typeof(BusinessApplicationContractsModule)
    )]
    public class BusinessHttpApiClientModule : AbpModule
    {
        public const string RemoteServiceName = "Business";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddHttpClientProxies(
                typeof(BusinessApplicationContractsModule).Assembly,
                RemoteServiceName
            );
        }
    }
}
