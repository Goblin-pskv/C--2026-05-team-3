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
        /// Получение списка мероприятий с пагинацией
        /// </summary>
        /// <param name="page">Номер страницы (по умолчанию 0)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
        /// <returns>Список мероприятий</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetEventsListQuery(
            [FromQuery] int page = 0, 
            [FromQuery] int pageSize = 10)
        {
            var query = new GetEventsListQuery(page, pageSize);
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
        /// <summary>
        /// Получение списка мероприятий организатора с пагинацией
        /// </summary>
        /// <param name="page">Номер страницы (по умолчанию 0)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyEventsQuery(
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 10)
        {
            Guid? organizerId = GetUserId();
            if (organizerId is null)
                return Unauthorized();
            var query = new GetMyEventsQuery((Guid)organizerId, page, pageSize);
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }
    }
}