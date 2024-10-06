using System.ComponentModel.DataAnnotations.Schema;

namespace Ticket.Entities
{
    [Table("msUser")]
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
