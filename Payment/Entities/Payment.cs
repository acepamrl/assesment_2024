using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.Entities
{
    [Table("trPayment")]
    public class Payment : BaseEntity
    {
        public Guid IdTicket { get; set; }
        public string Data { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
    }
}
