using Event.Models.Request;
using Event.Persistences;
using MassTransit;
using Ticket.MessageBroker.PublisherModels;

namespace Event.MessageBroker
{
    public class DecreaseEventTicketConsumer : IConsumer<DecreaseEventTicketModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public DecreaseEventTicketConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task Consume(ConsumeContext<DecreaseEventTicketModel> context)
        {
			try
			{
                var eventQuery = _dbContext.Events.FirstOrDefault(x => x.Id == context.Message.IdEvent && x.Status == Enums.EntityStatusEnum.Active);
                if (eventQuery == null)
                    throw new Exception("Event not found");

                eventQuery.Ticket--;
                _dbContext.Update(eventQuery);
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
