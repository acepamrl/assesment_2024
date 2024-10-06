using Event.Handlers;
using Event.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Event.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : Controller
    {
        private readonly IEventHandler _eventHandler;

        public EventController(IEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] DatatableRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _eventHandler.GetAll(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _eventHandler.Create(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _eventHandler.Update(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _eventHandler.Delete(id, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
