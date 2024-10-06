using Event.MessageBroker.PublisherModels;
using MassTransit;
using Ticket.Persistences;

namespace Ticket.MessageBroker
{
    public class UpdateEventConsumer : IConsumer<UpdateEventModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public UpdateEventConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task Consume(ConsumeContext<UpdateEventModel> context)
        {
            var eventQuery = _dbContext.Events.FirstOrDefault(x => x.Id == context.Message.Id && x.Status == Enums.EntityStatusEnum.Active);

            if (eventQuery == null)
                throw new Exception("Event not found");

            eventQuery.Name = context.Message.Name;
            eventQuery.Date = context.Message.Date;
            eventQuery.Location = context.Message.Location;
            eventQuery.Price = context.Message.Price;
            eventQuery.Ticket = context.Message.Ticket;
            eventQuery.UpdatedBy = context.Message.UpdatedBy;
            eventQuery.UpdatedDate = DateTime.UtcNow;

            _dbContext.Update(eventQuery);
            _dbContext.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
