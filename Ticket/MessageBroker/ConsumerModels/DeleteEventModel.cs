namespace Event.MessageBroker.PublisherModels
{
    public class DeleteEventModel
    {
        public Guid Id { get; set; }
        public string DeletedBy { get; set; } = string.Empty;
    }
}
