namespace Ticket.Models.Result
{
    public class GetTicketListResult
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime TicketBookingDate { get; set; }
        public string TicketStatus { get; set; } = string.Empty;
    }
}
