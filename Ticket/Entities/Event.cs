using System.ComponentModel.DataAnnotations.Schema;

namespace Ticket.Entities
{
    [Table("msEvent")]
    public class Event : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Ticket { get; set; }

        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
