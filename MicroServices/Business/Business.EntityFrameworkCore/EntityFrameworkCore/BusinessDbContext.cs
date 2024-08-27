using System.Collections.Generic;
using Business.Models;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Business.EntityFrameworkCore
{
    [ConnectionStringName("Business")]
    public class BusinessDbContext : AbpDbContext<BusinessDbContext>
    {
        public DbSet<Book> Book { get; set; }

        public DbSet<PrintTemplate> PrintTemplate { get; set; }

        // 自定義model
        public DbSet<Mission> Mission { get; set; }
        public DbSet<MissionI18N> MissionI18N { get; set; }
        public DbSet<MissionCategory> MissionCategory { get; set; }
        public DbSet<MissionCategoryI18N> MissionCategoryI18N { get; set; }
        public DbSet<MissionTag> MissionTag { get; set; }
        public DbSet<MissionTagI18N> MissionTagI18N { get; set; }
        public DbSet<MyFileInfo> MyFileInfo { get; set; }

        public DbSet<MissionView> MissionView { get; set; }
        public DbSet<MissionCategoryView> MissionCategoryView { get; set; }

        //Code generation...
        public BusinessDbContext(DbContextOptions<BusinessDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureBusiness();

            // 透過中間表維繫兩邊關聯，不手動寫可能會出錯(會一直去抓MissionMissionTag table)
            modelBuilder.Entity<Mission>()
                .HasMany(m => m.MissionTags)
                .WithMany(t => t.Missions)
                .UsingEntity<Dictionary<string, object>>(
                    "MissionPostTag", // 中间表名称
                    j => j.HasOne<MissionTag>().WithMany().HasForeignKey("MissionTagId"),
                    j => j.HasOne<Mission>().WithMany().HasForeignKey("MissionId")
                );

            // 自定義filter
            modelBuilder.Entity<Mission>(b =>
            {
                // 只有IsActive為true才會被包含在查詢範圍中
                b.HasAbpQueryFilter(e => e.IsActive);
            });
        }
    }
}