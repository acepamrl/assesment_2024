namespace Event.MessageBroker.PublisherModels
{
    public class NewEventModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Ticket { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
