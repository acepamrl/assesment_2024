using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.MessageBroker.PublisherModels;
using Payment.Models.Request;
using Payment.Persistences;
using Payment.Services;

namespace Payment.Handlers
{
    public interface IPaymentHandler
    {
        Task<bool> Pay(CreatePaymentRequest request, CancellationToken cancellationToken);
    }
    public class PaymentHandler : IPaymentHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;
        private readonly IBus _bus;

        public PaymentHandler(ApplicationDbContext dbContext, ICurrentUserService currentUser, IBus bus)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _bus = bus;
        }

        public async Task<bool> Pay(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var paymentQuery = await _dbContext.Payments.FirstOrDefaultAsync(x => x.IdTicket == request.IdTicket && x.Status == Enums.EntityStatusEnum.Active);

            if (paymentQuery == null) 
                throw new Exception("Payment data not found");
            if (paymentQuery.IsPaid)
                throw new Exception("Ticket has been paid");

            paymentQuery.IsPaid = true;
            paymentQuery.UpdatedBy = _currentUser.Id;
            paymentQuery.UpdatedDate = DateTime.UtcNow;

            _dbContext.Payments.Update(paymentQuery);

            await _dbContext.SaveChangesAsync(cancellationToken);

            #region Message Broker
            // Ticket
            // Update ticket status
            var ticketMessage = new PaidTicketModel
            {
                IdTicket = request.IdTicket
            };

            // Notification
            var notificationMessage = new SuccessPaymentNotificationModel
            {
                IdPayment = paymentQuery.Id,
                IdUser = Guid.Parse(_currentUser.Id),
                Data = paymentQuery.Data,
                NotificationType = Enums.NotificationTypeEnum.PushNotification
            };

            await _bus.Send(ticketMessage);
            await _bus.Send(notificationMessage);
            #endregion

            return true;
        }
    }
}
