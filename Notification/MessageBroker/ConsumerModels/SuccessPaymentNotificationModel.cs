using Notification.Enums;

namespace Payment.MessageBroker.PublisherModels
{
    public class SuccessPaymentNotificationModel
    {
        public Guid IdUser { get; set; }
        public Guid IdPayment { get; set; }
        public string Data { get; set; } = string.Empty;
        public NotificationTypeEnum NotificationType { get; set; }
    }
}
