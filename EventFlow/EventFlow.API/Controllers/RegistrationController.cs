using EventFlow.Application.Commands.EventCommands;
using EventFlow.Application.Commands.RegistrationCommands;
using EventFlow.Application.Common;
using EventFlow.Application.Queries.RegistrationQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventFlow.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Регистрация на мероприятие
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("{eventId}")]
        public async Task<IActionResult> CreateRegistrationCommand(Guid eventId, [FromBody] CreateRegistrationCommandMock command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Пользователь не авторизован");
            var commandWithGuid = command with { EventId = eventId, UserId = Guid.Parse(userId)};
            Result? result = await _mediator.Send(commandWithGuid);
            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                }
            }
            return Ok(result.Message);
        }
        /// <summary>
        /// Отмена регистрации на мероприятие
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> CancelRegistrationCommand(Guid eventId, [FromBody] CancelRegistrationCommandMock command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Пользователь не авторизован");
            var commandWithGuid = command with { EventId = eventId, UserId = Guid.Parse(userId) };
            Result? result = await _mediator.Send(commandWithGuid);
            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                }
            }
            return Ok(result.Message);
        }
        /// <summary>
        /// Получить список своих регистраций на мероприятия
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetUserRegistrationsQuery([FromQuery] GetUserRegistrationsQueryMock command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Пользователь не авторизован");
            Result? result = await _mediator.Send(new GetUserRegistrationsQueryWithUserIdMock(Guid.Parse(userId)));
            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                }
            }
            return Ok(result);
        }
        /// <summary>
        /// Получить список зарегестрировавшихся на мероприятие
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer, Admin")]
        [HttpGet("{eventId}/all")]
        public async Task<IActionResult> GetEventRegistrationsQuery(Guid eventId, [FromRoute] GetEventRegistrationsQueryMock command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Пользователь не авторизован");
            var commandWithGuid = command with { UserId = Guid.Parse(userId) };
            Result? result = await _mediator.Send(commandWithGuid);
            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                }
            }
            return Ok(result);
        }
    }
}