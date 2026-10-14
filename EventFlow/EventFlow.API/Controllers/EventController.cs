using EventFlow.Application.Commands.EventCommands;
using EventFlow.Application.Common;
using EventFlow.Application.Queries.EventQueries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventFlow.API.Controllers
{
    [Authorize(Roles = "Organizer, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : BaseController
    {
        public EventController(IMediator mediator) : base(mediator)
        {
        }
        /// <summary>
        /// Создание нового мероприятия
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateEventCommand([FromBody] CreateEventCommand command)
        {
            Result? result = await _mediator.Send(command);
            return HandleResult(result);
        }
        /// <summary>
        /// Получение данных о мероприятии
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventQuery(Guid eventId)
        {
            var query = new GetEventQuery { EventId = eventId };
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }
        /// <summary>
        /// Обновление данных мероприятия
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEventCommand (Guid eventId, [FromBody] UpdateEventCommand command)
        {
            var commandWithGuid = command with { EventId = eventId};
            Result? result = await _mediator.Send(commandWithGuid);
            return HandleResult(result);
        }
    }
}