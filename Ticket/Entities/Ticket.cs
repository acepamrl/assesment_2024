using System.ComponentModel.DataAnnotations.Schema;
using Ticket.Enums;

namespace Ticket.Entities
{
    [Table("trTicket")]
    public class Ticket : BaseEntity
    {
        public Guid IdEvent { get; set; }
        public Guid IdUser { get; set; }
        public TransactionStatusEnum TransactionStatus { get; set; }

        [ForeignKey(nameof(IdEvent))]
        public virtual Event Event { get; set; }
        [ForeignKey(nameof(IdUser))]
        public virtual User User { get; set; }
    }
}
