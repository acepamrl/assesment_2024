using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification.Handlers;
using Notification.Models.Request;

namespace Notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationHandler _notificationHandler;

        public NotificationController(INotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] DatatableRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _notificationHandler.GetAll(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
