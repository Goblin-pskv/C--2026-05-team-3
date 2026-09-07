using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Queries.GetProfileQuery;
using EventFlow.Application.Queries.LoginQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace EventFlow.API.Controllers {
    /// <summary>
    /// При помощи MediatR реализуем команды из EventFlow.Application.Commands
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
        }

        // POST: api/Auth
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUserCommand([FromBody] RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new {
                    Error = result.Error
                });

            return Ok(new {
                Message = "User registered successfully"
            });
        }

        // POST: api/Auth
        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileCommand([FromBody] UpdateProfileCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new {
                    Error = result.Error
                });

            return Ok(new {
                Message = "User profile updated successfully"
            });
        }

        // POST: api/Auth
        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLoginCommand([FromBody] LoginQuery command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new {
                    Error = result.Error
                });

            return Ok(new {
                Message = "Login Succeded"
            });
        }

        // POST: api/Auth
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfileCommand([FromBody] GetProfileQuery command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new {
                    Error = result.Error
                });

            return Ok(result);
        }
    }
}


