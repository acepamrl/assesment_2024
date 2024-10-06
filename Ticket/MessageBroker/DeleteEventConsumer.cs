using Event.MessageBroker.PublisherModels;
using MassTransit;
using Ticket.Persistences;

namespace Ticket.MessageBroker
{
    public class DeleteEventConsumer : IConsumer<DeleteEventModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteEventConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<DeleteEventModel> context)
        {
            var eventQuery = _dbContext.Events.FirstOrDefault(x => x.Id == context.Message.Id && x.Status == Enums.EntityStatusEnum.Active);

            if (eventQuery == null)
                throw new Exception("Event not found");

            eventQuery.Status = Enums.EntityStatusEnum.NonActive;
            eventQuery.DeletedBy = context.Message.DeletedBy;
            eventQuery.DeletedDate = DateTime.UtcNow;

            _dbContext.Update(eventQuery);
            _dbContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
