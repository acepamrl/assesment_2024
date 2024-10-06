namespace Ticket.MessageBroker.PublisherModels
{
    public class CreatePaymentModel
    {
        public Guid IdTicket { get; set; }
        public Guid IdUser { get; set; }
        public double Price { get; set; }
        public string Data { get; set; } = string.Empty;
    }
}
