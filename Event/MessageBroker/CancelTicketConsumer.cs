using Event.Persistences;
using MassTransit;
using Ticket.MessageBroker.PublisherModels;

namespace Event.MessageBroker
{
    public class CancelTicketConsumer : IConsumer<CancelTicketModel>
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;

        public CancelTicketConsumer(ApplicationDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<CancelTicketModel> context)
        {
            try
            {
                var eventQuery = _dbContext.Events.FirstOrDefault(x => x.Id == context.Message.IdEvent && x.Status == Enums.EntityStatusEnum.Active);
                if (eventQuery == null)
                    throw new Exception("Event not found");

                eventQuery.Ticket++;
                _dbContext.Update(eventQuery);
                _dbContext.SaveChanges();

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new Exception(e.Message);
            }
        }
    }
}
