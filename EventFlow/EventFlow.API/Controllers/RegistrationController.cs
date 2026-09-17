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
        public async Task<IActionResult> CreateRegistrationCommand(Guid eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userId, out Guid userGuid))
                return Unauthorized("Пользователь не авторизован");
            var command = new CreateRegistrationCommand(eventId, userGuid);
            Result? result = await _mediator.Send(command);
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
        public async Task<IActionResult> CancelRegistrationCommand(Guid eventId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
                return Unauthorized("Пользователь не авторизован");
            var command = new CancelRegistrationCommand(eventId, userGuid);
            Result? result = await _mediator.Send(command);
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
        public async Task<IActionResult> GetUserRegistrationsQuery()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid userGuid))
                return Unauthorized("Пользователь не авторизован");

            var query = new GetUserRegistrationsQuery(userGuid);
            var result = await _mediator.Send(query);

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
        public async Task<IActionResult> GetEventRegistrationsQuery(Guid eventId)
        {
            var query = new GetEventRegistrationsQuery(eventId);
            var result = await _mediator.Send(query);

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
