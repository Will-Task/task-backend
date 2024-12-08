using Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using XCZ.EntityFrameworkCore;

namespace Business.EntityFrameworkCore
{
    [DependsOn(
        typeof(BusinessDomainModule),
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(FormEntityFrameworkCoreModule),
        typeof(FlowEntityFrameworkCoreModule)
    )]
    public class BusinessEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpDbContextOptions>(options =>
            {
                options.UseSqlServer();
            });

            context.Services.AddAbpDbContext<BusinessDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
                
                // 配置Eager Loading
                options.Entity<Mission>(
                    x =>
                    {
                        x.DefaultWithDetailsFunc = query => query.Include(m => m.MissionI18Ns);
                    }
                );
                
                options.Entity<MissionCategory>(
                    x =>
                    {
                        x.DefaultWithDetailsFunc = query => query.Include(m => m.MissionCategoryI18Ns);
                    }
                );
            });
        }
    }
}
