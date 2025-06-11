using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Notification.notify;
using EasyAbp.NotificationService.Notifications;
using EasyAbp.NotificationService.Provider.PrivateMessaging;
using Volo.Abp.DependencyInjection;

namespace Business.Notification.factory
{
    public class MessageNotificationFactory : NotificationFactory<MessageNotification, CreatePrivateMessageNotificationEto>, ITransientDependency
    {

        public override Task<CreatePrivateMessageNotificationEto> CreateAsync(MessageNotification model, IEnumerable<NotificationUserInfoModel> users)
        {
            var text = $"Hello, {model.UserName}, here is a gift card code for you: ";
            var userIds = users.Select(x => x.Id);

            return Task.FromResult(new CreatePrivateMessageNotificationEto(null, userIds, text, string. Empty, true));
        }

        public override Task<CreatePrivateMessageNotificationEto> CreateAsync(MessageNotification model, IEnumerable<Guid> userIds)
        {
            var text = $"Hello, {model.UserName}, here is a gift card code for you: ";
            return Task.FromResult(new CreatePrivateMessageNotificationEto(null, userIds, text, string.Empty, true));
        }
    }
}
