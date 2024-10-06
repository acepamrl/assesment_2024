using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ticket.Handlers;
using Ticket.Models.Request;

namespace Ticket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketHandler _ticketHandler;

        public TicketController(ITicketHandler ticketHandler)
        {
            _ticketHandler = ticketHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] DatatableRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _ticketHandler.GetAll(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _ticketHandler.Create(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelTicket(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _ticketHandler.Cancel(id, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
