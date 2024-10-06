using MassTransit;
using Payment.Persistences;
using Ticket.MessageBroker.PublisherModels;

namespace Payment.MessageBroker
{
    public class CancelPaymentConsumer : IConsumer<CancelPaymentModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public CancelPaymentConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<CancelPaymentModel> context)
        {
            try
            {
                var paymentQuery = _dbContext.Payments.FirstOrDefault(x => x.IdTicket == context.Message.IdTicket && x.Status == Enums.EntityStatusEnum.Active);

                if (paymentQuery == null)
                    throw new Exception("Payment data not found");

                paymentQuery.Status = Enums.EntityStatusEnum.NonActive;
                paymentQuery.DeletedBy = "system";
                paymentQuery.DeletedDate = DateTime.UtcNow;

                _dbContext.Update(paymentQuery);
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
