using System.Threading.Tasks;
using Business.Common;
using Business.Models;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Business.Hangfire
{
    [Queue("alpha")]
    public class BackgroundJobManager : AsyncBackgroundJob<EmailArg>, ITransientDependency
    {
        private EmailUtils emailUtils;
        private IConfiguration configuration;
        private IBackgroundJobManager backgroundJobManager;

        public BackgroundJobManager(EmailUtils _emailUtils, IConfiguration _configuration, IBackgroundJobManager _backgroundJobManager)
        {
            emailUtils = _emailUtils;
            configuration = _configuration;
            backgroundJobManager = _backgroundJobManager;
        }

        public override async Task ExecuteAsync(EmailArg setting)
        {
            await emailUtils.SendAsync(setting.To, setting.Subject, setting.Body);
        }
    }
}
