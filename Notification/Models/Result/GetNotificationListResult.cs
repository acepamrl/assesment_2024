namespace Notification.Models.Result
{
    public class GetNotificationListResult
    {
        public Guid Id { get; set; }
        public Guid IdPayment { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public NotificationDataModel Data { get; set; } = new NotificationDataModel();
        public bool IsRead { get; set; }
    }

    public class NotificationDataModel
    {

    }
}
