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
    public class EventController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Создание нового мероприятия
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateEventCommand([FromBody] CreateEventCommandMock command)
        {
            Result? result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 422:
                        return UnprocessableEntity(result.Message);
                }
            }
            return Ok(result.Message);
        }
        /// <summary>
        /// Получение данных о мероприятии
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEventQuery(Guid eventId, GetEventQueryMock command)
        {
            var commandWithGuid = command with { EventId = eventId };
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
        /// Обновление данных мероприятия
        /// </summary>
        /// <param name="eventId">ID мероприятия</param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{eventId}")]
        public async Task<IActionResult> UpdateEventCommand (Guid eventId, [FromBody] UpdateEventCommandMock command)
        {
            var commandWithGuid = command with { EventId = eventId};
            Result? result = await _mediator.Send(commandWithGuid);
            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 404:
                        return NotFound(result.Message);
                    case 422:
                        return UnprocessableEntity(result.Message);
                }
            }
            return Ok(result.Message);
        }
    }
}