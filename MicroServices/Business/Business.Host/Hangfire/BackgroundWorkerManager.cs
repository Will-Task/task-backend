using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Business.MissionManagement;
using Hangfire;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace Business.Hangfire
{
    public class BackgroundWorkerManager : HangfireBackgroundWorkerBase
    {
        private IMissionAppService missionAppService;
        public BackgroundWorkerManager(IMissionAppService _missionAppService)
        {
            RecurringJobId = nameof(BackgroundWorkerManager);
            CronExpression = Cron.MinuteInterval(1);
            missionAppService = _missionAppService;
        }

        public override Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation("Executed MyLogWorker..!");
            missionAppService.MissionReminder();
            return Task.CompletedTask;
        }
    }
}
