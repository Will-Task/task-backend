using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Business.EntityFrameworkCore;

public class BusinessDbContextFactory : IDesignTimeDbContextFactory<BusinessDbContext>
{
    public BusinessDbContext CreateDbContext(string[] args)
    {   
        var configuration = BuildConfiguration();
        var optionsBuilder = new DbContextOptionsBuilder<BusinessDbContext>();
        
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Business"));

        return new BusinessDbContext(optionsBuilder.Options);
    }
    
    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}