using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EventFlow.API.Controllers
{
    /// <summary>
    /// При помощи MediatR реализуем команды из EventFlow.Application.Commands
    /// </summary>

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        // POST: api/events
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUserCommand(RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return Ok(result);
        }

        // POST: api/events
        [HttpPost("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileCommand(UpdateProfileCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(new { Error = result.Error });

            return Ok(new { Message = "User profile updated successfully" });
        }




    }
}


