namespace Event.Models.Request
{
    public class CreateTicketRequest
    {
        public Guid IdEvent { get; set; }
        public int AvailableTicket { get; set; }
    }
}
