using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using System.Linq;
using Ticket.Entities;
using Ticket.Helper;
using Ticket.MessageBroker.PublisherModels;
using Ticket.Models.Request;
using Ticket.Models.Result;
using Ticket.Persistences;
using Ticket.Services;

namespace Ticket.Handlers
{
    public interface ITicketHandler
    {
        Task<DatatableResult> GetAll(DatatableRequest request, CancellationToken cancellationToken);
        Task<bool> Create(CreateTicketRequest request, CancellationToken cancellationToken);
        Task<bool> Cancel(Guid idTicket, CancellationToken cancellationToken);
    }
    public class TicketHandler : ITicketHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;
        private readonly IBus _bus;

        public TicketHandler(ApplicationDbContext dbContext, ICurrentUserService currentUser, IBus bus)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
            _bus = bus;
        }

        public async Task<DatatableResult> GetAll(DatatableRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Tickets
                .Include(x => x.Event)
                .Where(x => x.Status == Enums.EntityStatusEnum.Active && x.IdUser == Guid.Parse(_currentUser.Id)).AsQueryable();

            var totalRecord = query.Count();

            switch (request.OrderCol)
            {
                case "event_name":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Event.Name) :
                        query.OrderByDescending(x => x.Event.Name);
                    break;
                case "event_date":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Event.Date) :
                        query.OrderByDescending(x => x.Event.Date);
                    break;
                case "location":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Event.Location) :
                        query.OrderByDescending(x => x.Event.Location);
                    break;
                case "ticket_booking_date":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.CreatedDate) :
                        query.OrderByDescending(x => x.CreatedDate);
                    break;
                case "status":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Status) :
                        query.OrderByDescending(x => x.Status);
                    break;
                default:
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.CreatedDate) :
                        query.OrderByDescending(x => x.CreatedDate);
                    break;
            }

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.Event.Name.ToLower().Contains(request.Keyword.ToLower()));

            var data = await query.Skip(request.Start).Take(request.Length)
                    .Select(x => new GetTicketListResult
                    {
                        Id = x.Id,
                        EventName = x.Event.Name,
                        EventDate = x.Event.Date,
                        Location = x.Event.Location,
                        TicketBookingDate = x.CreatedDate,
                        TicketStatus = EnumHelper.GetEnumDescription(x.TransactionStatus)
                    }).ToListAsync(cancellationToken);

            if (data == null || !data.Any())
            {
                return new DatatableResult
                {
                    RecordsFiltered = 0,
                    RecordsTotal = 0,
                    Data = null,
                };
            }

            return new DatatableResult
            {
                RecordsTotal = totalRecord,
                RecordsFiltered = data.Count(),
                Data = data
            };
        }

        public async Task<bool> Create(CreateTicketRequest request, CancellationToken cancellationToken)
        {
            if (request.IdEvent == Guid.Empty)
                throw new Exception("Id event cannot be null");

            var eventQuery = _dbContext.Events.FirstOrDefaultAsync(x => x.Id == request.IdEvent && x.Status == Enums.EntityStatusEnum.Active, cancellationToken);
            if (eventQuery == null)
                throw new Exception("Event not found");
            if (eventQuery.Result!.Ticket - 1 < 0)
                throw new Exception("Ticket sold out");
            if (await _dbContext.Tickets.AnyAsync(x => x.IdEvent == request.IdEvent && x.IdUser == Guid.Parse(_currentUser.Id) && x.Status == Enums.EntityStatusEnum.Active, cancellationToken))
                throw new Exception("Ticket is exist");

            var newTicket = new Entities.Ticket
            {
                Id = Guid.NewGuid(),
                IdEvent = request.IdEvent,
                IdUser = Guid.Parse(_currentUser.Id),
                TransactionStatus = Enums.TransactionStatusEnum.WaitingPayment,
                CreatedBy = _currentUser.Id,
                CreatedDate = DateTime.UtcNow,
            };

            await _dbContext.AddAsync(newTicket, cancellationToken);

            eventQuery.Result.Ticket--;
            _dbContext.Events.Update(eventQuery.Result);

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Message broker
            // Decrease total ticket from event
            var eventMessage = new DecreaseEventTicketModel
            {
                IdEvent = request.IdEvent
            };

            var eventData = new
            {
                eventQuery.Id,
                eventQuery.Result.Name,
                eventQuery.Result.Location,
                eventQuery.Result.Date,
                eventQuery.Result.Price
            };

            // Create new ticket if not exist & new payment
            var paymentMessage = new CreatePaymentModel
            {
                IdTicket = newTicket.Id,
                IdUser = Guid.Parse(_currentUser.Id),
                Price = eventQuery.Result!.Price,
                Data = JsonConvert.SerializeObject(eventData)
            };

            // Send notification to user


            await _bus.Publish(eventMessage);
            await _bus.Publish(paymentMessage);

            return true;
        }

        public async Task<bool> Cancel(Guid idTicket, CancellationToken cancellationToken)
        {
            var ticketQuery = await _dbContext.Tickets.Include(x => x.Event).FirstOrDefaultAsync(x => x.Id == idTicket && x.Status == Enums.EntityStatusEnum.Active, cancellationToken);

            if (ticketQuery == null || ticketQuery.TransactionStatus == Enums.TransactionStatusEnum.Cancelled)
                throw new Exception("Ticket not found");

            if (ticketQuery.TransactionStatus == Enums.TransactionStatusEnum.Success)
                throw new Exception("Cannot cancel ticket. Tickets have been paid.");

            ticketQuery.Status = Enums.EntityStatusEnum.NonActive;
            ticketQuery.TransactionStatus = Enums.TransactionStatusEnum.Cancelled;

            ticketQuery.Event.Ticket++;

            _dbContext.Update(ticketQuery);
            _dbContext.Update(ticketQuery.Event);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // Message Broker
            var eventMessage = new CancelTicketModel
            {
                IdEvent = ticketQuery.IdEvent
            };

            // Delete payment data
            var paymentMessage = new CancelPaymentModel
            {
                IdTicket = ticketQuery.Id
            };

            await _bus.Publish(eventMessage);
            await _bus.Publish(paymentMessage);

            return true;
        }
    }
}
