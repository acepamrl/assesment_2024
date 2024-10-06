using MassTransit;
using Payment.Persistences;
using Ticket.MessageBroker.PublisherModels;

namespace Payment.MessageBroker
{
    public class CreatePaymentConsumer : IConsumer<CreatePaymentModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public CreatePaymentConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<CreatePaymentModel> context)
        {
            try
            {
                var newPayment = new Entities.Payment
                {
                    Id = Guid.NewGuid(),
                    IdTicket = context.Message.IdTicket,
                    IsPaid = false,
                    CreatedBy = "system",
                    CreatedDate = DateTime.UtcNow,
                };

                _dbContext.Payments.Add(newPayment);
                _dbContext.SaveChanges();

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
