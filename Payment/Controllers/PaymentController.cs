using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Handlers;
using Payment.Models.Request;

namespace Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentHandler _paymentHandler;
        public PaymentController(IPaymentHandler paymentHandler)
        {
            _paymentHandler = paymentHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Pay(CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _paymentHandler.Pay(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
