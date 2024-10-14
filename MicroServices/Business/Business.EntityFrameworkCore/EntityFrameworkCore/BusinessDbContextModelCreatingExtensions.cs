using Business.Models;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Business.EntityFrameworkCore
{
    public static class BusinessDbContextModelCreatingExtensions
    {
        public static void ConfigureBusiness(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
            //Code generation...
        }
    }
}
