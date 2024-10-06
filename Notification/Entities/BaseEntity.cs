using Notification.Enums;
namespace Notification.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Status = EntityStatusEnum.Active;
        }

        public Guid Id { get; set; }
        public EntityStatusEnum Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
        public DateTime? DeletedDate { get; set; }
    }
}
