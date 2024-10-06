namespace Event.MessageBroker.PublisherModels
{
    public class UpdateEventModel : NewEventModel
    {
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
