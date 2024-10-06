using Event.MessageBroker.PublisherModels;
using Event.Models.Request;
using Event.Models.Result;
using Event.Persistences;
using Event.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Event.Handlers
{
    public interface IEventHandler
    {
        Task<DatatableResult> GetAll(DatatableRequest request, CancellationToken cancellationToken);
        Task<bool> Create(CreateEventRequest request, CancellationToken cancellationToken);
        Task<bool> Update(UpdateEventRequest request, CancellationToken cancellationToken);
        Task<bool> Delete(Guid id, CancellationToken cancellationToken);
    }

    public class EventHandler : IEventHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IBus _bus;
        private readonly ICurrentUserService _currentUser;

        public EventHandler(ApplicationDbContext dbContext, ICurrentUserService currentUser, IBus bus)
        {
            _dbContext = dbContext;
            _bus = bus;
            _currentUser = currentUser;
        }

        public async Task<DatatableResult> GetAll(DatatableRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Events.Where(x => x.Status == Enums.EntityStatusEnum.Active).AsQueryable();

            var totalRecord = query.Count();

            switch (request.OrderCol)
            {
                case "name":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Name) :
                        query.OrderByDescending(x => x.Name);
                    break;
                case "date":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Date) :
                        query.OrderByDescending(x => x.Date);
                    break;
                case "location":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Location) :
                        query.OrderByDescending(x => x.Location);
                    break;
                case "ticket":
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.Ticket) :
                        query.OrderByDescending(x => x.Ticket);
                    break;
                default:
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.CreatedDate) :
                        query.OrderByDescending(x => x.CreatedDate);
                    break;
            }

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.Name.ToLower().Contains(request.Keyword.ToLower()));

            var data = await query.Skip(request.Start).Take(request.Length)
                    .Select(x => new GetEventListModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Date = x.Date,
                        Location = x.Location,
                        Ticket = x.Ticket
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

        public async Task<bool> Create(CreateEventRequest request, CancellationToken cancellationToken)
        {
            if (request.Ticket <= 0)
                throw new Exception("Ticket number cannot be empty");

            var newEvent = new Entities.Event
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Date = request.Date,
                Location = request.Location,
                Price = request.Price,
                Ticket = request.Ticket,
                CreatedBy = _currentUser.Id,
                CreatedDate = DateTime.UtcNow,
            };

            await _dbContext.AddAsync(newEvent, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            #region Message Broker
            var eventMessage = new NewEventModel
            {
                Id = newEvent.Id,
                Name = newEvent.Name,
                Date = newEvent.Date,
                Location = newEvent.Location,
                Price = newEvent.Price,
                Ticket = newEvent.Ticket,
                CreatedBy = _currentUser.Id
            };
            await _bus.Publish(eventMessage);

            #endregion
            return true;
        }

        public async Task<bool> Update(UpdateEventRequest request, CancellationToken cancellationToken)
        {
            var eventQuery = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == request.Id && x.Status == Enums.EntityStatusEnum.Active, cancellationToken);
            if (eventQuery == null)
                throw new Exception("Event not found");

            if (request.Ticket < eventQuery.TicketSold)
                throw new Exception("Cannot update event. Ticket sold greater than available ticket.");

            eventQuery.Name = request.Name;
            eventQuery.Date = request.Date;
            eventQuery.Location = request.Location;
            eventQuery.Price = request.Price;
            eventQuery.Ticket = request.Ticket;

            _dbContext.Update(eventQuery);
            await _dbContext.SaveChangesAsync(cancellationToken);

            #region Message Broker
            var eventMessage = new UpdateEventModel
            {
                Id = eventQuery.Id,
                Name = eventQuery.Name,
                Date = eventQuery.Date,
                Location = eventQuery.Location,
                Price = eventQuery.Price,
                Ticket = eventQuery.Ticket,
                UpdatedBy = _currentUser.Id
            };
            await _bus.Publish(eventMessage);
            #endregion

            return true;
        }

        public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
        {
            var eventQuery = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == id && x.Status == Enums.EntityStatusEnum.Active, cancellationToken);
            if (eventQuery == null)
                throw new Exception("Event not found");

            if (eventQuery.TicketSold > 0)
                throw new Exception("Cannot delete event. Tickets have been sold");

            eventQuery.Status = Enums.EntityStatusEnum.NonActive;
            eventQuery.DeletedBy = _currentUser.Id;
            eventQuery.DeletedDate = DateTime.UtcNow;

            _dbContext.Update(eventQuery);
            await _dbContext.SaveChangesAsync(cancellationToken);

            #region Message Broker
            var eventMessage = new DeleteEventModel
            {
                Id = eventQuery.Id,
                DeletedBy = _currentUser.Id
            };
            await _bus.Publish(eventMessage);
            #endregion

            return true;
        }
    }
}
