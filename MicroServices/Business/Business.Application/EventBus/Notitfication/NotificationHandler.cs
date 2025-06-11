using System.Threading.Tasks;
using EasyAbp.NotificationService.Provider.PrivateMessaging;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Business.EventBus.Notitfication;

public class NotificationHandler: IDistributedEventHandler<CreatePrivateMessageNotificationEto>,
    ITransientDependency
{
    private ILogger<NotificationHandler> _logger;

    public NotificationHandler(ILogger<NotificationHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleEventAsync(CreatePrivateMessageNotificationEto eventData)
    {
        _logger.LogError(eventData.Title);
    }
}