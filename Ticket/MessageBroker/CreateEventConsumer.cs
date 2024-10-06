using Event.MessageBroker.PublisherModels;
using MassTransit;
using Ticket.Persistences;

namespace Ticket.MessageBroker
{
    public class CreateEventConsumer : IConsumer<NewEventModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateEventConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<NewEventModel> context)
        {
            try
            {
                var newEvent = new Ticket.Entities.Event
                {
                    Id = context.Message.Id,
                    Name = context.Message.Name,
                    Date = context.Message.Date,
                    Location = context.Message.Location,
                    Price = context.Message.Price,
                    Ticket = context.Message.Ticket,
                    CreatedBy = context.Message.CreatedBy,
                    CreatedDate = DateTime.UtcNow
                };

                _dbContext.Add(newEvent);
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
