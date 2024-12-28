using System;
using System.IO;
using System.Threading.Tasks;
using AuthServer.Encryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Business
{
    public class Program
    {
        public static Stream JsonStream = null;
        public async static Task<int> Main(string[] args)
        {
            if (AppsettingHandler.IsExistAppsettings())
            {
                await AppsettingHandler.EncryptionAsync();
            }

            JsonStream = await AppsettingHandler.GetDecryptionStreamAsync();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting Business.Host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                JsonStream.Dispose();
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration(config => config.AddJsonStream(JsonStream));
                    webBuilder.UseStartup<Startup>();
                })
                .UseAutofac()
                .UseSerilog();
    }
}
