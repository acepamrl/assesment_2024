using MassTransit;
using Payment.MessageBroker.PublisherModels;
using Ticket.Persistences;

namespace Ticket.MessageBroker
{
    public class PaidTicketConsumer : IConsumer<PaidTicketModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public PaidTicketConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<PaidTicketModel> context)
        {
            try
            {
                var ticketQuery = _dbContext.Tickets.FirstOrDefault(x => x.Id == context.Message.IdTicket && x.Status == Enums.EntityStatusEnum.Active);
                if (ticketQuery == null)
                    throw new Exception("Ticket not found");

                ticketQuery.TransactionStatus = Enums.TransactionStatusEnum.Success;
                ticketQuery.UpdatedBy = "system";
                ticketQuery.UpdatedDate = DateTime.UtcNow;

                _dbContext.Tickets.Update(ticketQuery);
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
