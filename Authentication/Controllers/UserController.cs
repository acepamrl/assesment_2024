using Authentication.Handlers;
using Authentication.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserHandler _userHandler;

        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _userHandler.Create(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Auth([FromBody] AuthRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _userHandler.Login(request, cancellationToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
