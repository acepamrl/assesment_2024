namespace Ticket.MessageBroker.PublisherModels
{
    public class CreatePaymentNotificationModel
    {
        public Guid IdEvent { get; set; } = Guid.NewGuid();
    }
}
