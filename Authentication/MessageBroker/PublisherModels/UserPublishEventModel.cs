namespace Authentication.MessageBroker.PublisherModels
{
    public class UserPublishEventModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
