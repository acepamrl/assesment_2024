using System.ComponentModel.DataAnnotations.Schema;

namespace Event.Entities
{
    [Table("msEvent")]
    public class Event : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Ticket { get; set; }
        public int TicketSold { get; set; }
    }
}
