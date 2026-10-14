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
    public class RegistrationController : BaseController
    {
        public RegistrationController(IMediator mediator) : base(mediator)
        {
        }
        /// <summary>
        /// Регистрация на мероприятие
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [HttpPost("{eventId}")]
        public async Task<IActionResult> CreateRegistrationCommand(Guid eventId)
        {
            var userId = GetUserId();
            
            if (userId == null)
                return Unauthorized("Пользователь не авторизован");
                
            var command = new CreateRegistrationCommand(eventId, userId.Value);
            Result? result = await _mediator.Send(command);
            return HandleResult(result);
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
            var userId = GetUserId();
            
            if (userId == null)
                return Unauthorized("Пользователь не авторизован");
                
            var command = new CancelRegistrationCommand(eventId, userId.Value);
            Result? result = await _mediator.Send(command);
            return HandleResult(result);
        }
        /// <summary>
        /// Получить список своих регистраций на мероприятия
        /// </summary>
        /// <returns></returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetUserRegistrationsQuery()
        {
            var userId = GetUserId();
            
            if (userId == null)
                return Unauthorized("Пользователь не авторизован");

            var query = new GetUserRegistrationsQuery(userId.Value);
            var result = await _mediator.Send(query);

            return HandleResult(result);
        }
        /// <summary>
        /// Получить список зарегестрировавшихся на мероприятие
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer, Admin")]
        [HttpGet("{eventId}/all")]
        public async Task<IActionResult> GetEventRegistrationsQuery(Guid eventId)
        {
            var query = new GetEventRegistrationsQuery(eventId);
            var result = await _mediator.Send(query);

            return HandleResult(result);
        }
    }
}
