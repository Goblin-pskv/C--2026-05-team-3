using EventFlow.Application.Queries.TicketQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;

namespace EventFlow.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : BaseController
    {
        public TicketController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Возвращает билет по идентификатору билета
        /// </summary>
        /// <param name="id">Идентификатор билета</param>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTicketQuery(Guid id)
        {
            var query = new GetTicketQuery(id);
            var result = await _mediator.Send(query);

            return HandleResult(result);
        }

        /// <summary>
        /// Возвращает список билетов авторизованного пользователя
        /// </summary>
        /// <returns></returns>
        [HttpGet("~/api/users/me/tickets")]
        public async Task<IActionResult> GetMyTicketsQuery()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized("Пользователь не авторизован");

            var query = new GetMyTicketsQuery(userId.Value);

            var result = await _mediator.Send(query);

            return HandleResult(result);
        }

        /// <summary>
        /// Возвращает список билетов по Id мероприятия
        /// </summary>
        /// <param name="eventId">Id мероприятия</param>
        /// <returns></returns>
        [Authorize(Roles = "Organizer, Admin")]
        [HttpGet("{eventId}/all")]
        public async Task<IActionResult> GetTicketsEventQuery(Guid eventId)
        {
            var query = new GetTicketsEventQuery(eventId);
            var result = await _mediator.Send(query);

            return HandleResult(result);
        }

        
    }
}

