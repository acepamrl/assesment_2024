namespace Event.Models.Request
{
    public class UpdateEventRequest : CreateEventRequest
    {
        public Guid Id { get; set; }
    }
}
