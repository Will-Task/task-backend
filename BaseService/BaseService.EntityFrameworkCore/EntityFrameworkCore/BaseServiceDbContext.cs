using System;
using System.Collections.Generic;
using BaseService.BaseData;
using BaseService.Systems;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;

namespace BaseService.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class BaseServiceDbContext : AbpDbContext<BaseServiceDbContext>
    {
        public DbSet<DataDictionary> DataDictionaries { get; set; }

        public DbSet<DataDictionaryDetail> DataDictionaryDetails { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<UserJob> UserJobs { get; set; }

        public DbSet<UserOrganization> UserOrganizations { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public DbSet<RoleMenu> RoleMenus { get; set; }
        
        public DbSet<Team> Team { get; set; }
        
        public DbSet<TeamView> TeamView { get; set; }
        
        public DbSet<TeamMission> TeamMission { get; set; }

        public BaseServiceDbContext(DbContextOptions<BaseServiceDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //builder.Entity<AppUser>(b =>
            //{
            //    b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users"); //Sharing the same table "AbpUsers" with the IdentityUser

            //    b.ConfigureByConvention();
            //    b.ConfigureAbpUser();

            //    b.Property(x => x.Enable).HasDefaultValue(true);

            //}); 
            
            // 要設定，不然TeamMission會報錯
            builder.Entity<TeamMission>().HasKey(c => new { c.TeamId, c.UserId });
            builder.ConfigureBaseService();
        }
    }
}
