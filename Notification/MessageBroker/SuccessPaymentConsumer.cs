using MassTransit;
using Notification.Entities;
using Notification.Persistences;
using Payment.MessageBroker.PublisherModels;

namespace Notification.MessageBroker
{
    public class SuccessPaymentConsumer : IConsumer<SuccessPaymentNotificationModel>
    {
        private readonly ApplicationDbContext _dbContext;

        public SuccessPaymentConsumer(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Consume(ConsumeContext<SuccessPaymentNotificationModel> context)
        {
            try
            {
                var newNotification = new Entities.Notification
                {
                    Id = Guid.NewGuid(),
                    IdPayment = context.Message.IdPayment,
                    IdUser = context.Message.IdUser,
                    Data = context.Message.Data,
                    NotificationType = context.Message.NotificationType,
                    CreatedBy = "system",
                    CreatedDate = DateTime.UtcNow,
                };

                _dbContext.Add(newNotification);
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
