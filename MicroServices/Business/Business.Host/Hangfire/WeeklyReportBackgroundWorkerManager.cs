using System;
using System.Threading;
using System.Threading.Tasks;
using Business.MissionManagement;
using Hangfire;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace Business.Hangfire
{
    public class WeeklyReportBackgroundWorkerManager : HangfireBackgroundWorkerBase
    {
        private IMissionAppService missionAppService;
        public WeeklyReportBackgroundWorkerManager(IMissionAppService _missionAppService)
        {
            RecurringJobId = nameof(WeeklyReportBackgroundWorkerManager);
            
            CronExpression = Cron.Weekly(DayOfWeek.Saturday);
            missionAppService = _missionAppService;
        }

        public override Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Executed WeeklyReportBackgroundJobManager..!");
            Logger.LogInformation($"{DateTime.UtcNow}");
            missionAppService.ExportReport();
            return Task.CompletedTask;
        }
    }
}