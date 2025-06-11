using System;
using System.Collections.Generic;
using EasyAbp.NotificationService.Notifications;
using Volo.Abp.MultiTenancy;

namespace Business.Notification.notify
{
    public class MessageNotificationEto : CreateNotificationInfoModel, IMultiTenant
    {
        public string Text { get; set; }

        public Guid? TenantId {  get; set; }

        /*
            | 值（string）   | 代表的意思            |
            | ----------- | ---------------- |
            | `"Email"`   | 用電子郵件寄送通知        |
            | `"Sms"`     | 發送簡訊             |
            | `"SignalR"` | 即時推播給線上用戶        |
            | `"Webhook"` | 呼叫外部 Webhook API |
            | `"InApp"`   | 系統內建通知訊息         |
        */

        public const string NotificationMethod = "PM";

        public MessageNotificationEto(IEnumerable<Guid> userIds, string text): base(NotificationMethod, userIds)
        {
            Text = text;
        }
    }
}
