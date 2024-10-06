using Notification.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notification.Entities
{
    [Table("TrNotification")]
    public class Notification : BaseEntity
    {
        public Guid IdUser { get; set; }
        public Guid IdPayment { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
        public string Data { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
