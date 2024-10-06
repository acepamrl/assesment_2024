using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Notification.Helpers;
using Notification.Models.Request;
using Notification.Models.Result;
using Notification.Persistences;
using Notification.Services;

namespace Notification.Handlers
{
    public interface INotificationHandler
    {
        Task<DatatableResult> GetAll(DatatableRequest request, CancellationToken cancellationToken);
    }

    public class NotificationHandler : INotificationHandler
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUser;

        public NotificationHandler(ApplicationDbContext dbContext, ICurrentUserService currentUser)
        {
            _dbContext = dbContext;
            _currentUser = currentUser;
        }

        public async Task<DatatableResult> GetAll(DatatableRequest request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Notifications.Where(x => x.Status == Enums.EntityStatusEnum.Active && x.IdUser == Guid.Parse(_currentUser.Id)).AsQueryable();

            var totalRecord = query.Count();

            switch (request.OrderCol)
            {
                default:
                    query = request.OrderType.ToLower() == "asc" ?
                        query.OrderBy(x => x.CreatedDate) :
                        query.OrderByDescending(x => x.CreatedDate);
                    break;
            }


            var data = await query.Skip(request.Start).Take(request.Length)
                    .Select(x => new GetNotificationListResult
                    {
                        Id = x.Id,
                        IdPayment = x.IdPayment,
                        NotificationType = EnumHelper.GetEnumDescription(x.NotificationType),
                        Data = JsonConvert.DeserializeObject<NotificationDataModel>(x.Data)!,
                        IsRead = x.IsRead
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
    }
}
