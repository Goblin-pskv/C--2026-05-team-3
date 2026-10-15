using EventFlow.Application.Commands.OrganizerCommands;
using EventFlow.Application.Common;
using EventFlow.Application.Queries.OrganizerQueries;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizerController : BaseController
    {
        public OrganizerController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Подать заявку на организатора
        /// </summary>
        /// <param name="command">Данные для подачи заявки</param>
        /// <returns>Результат операции</returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrganizerCommand([FromBody] CreateOrganizerCommand command)
        {
            Guid? userId = GetUserId();
            if (userId is null)
                return Unauthorized();
            var commandWith = command with { userId = (Guid)userId};
            var result = await _mediator.Send(commandWith);
            return HandleResult(result);
        }

        /// <summary>
        /// Получить свою заявку на организатора
        /// </summary>
        /// <returns>Данные заявки</returns>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyOrganizerCommand()
        {
            Guid? userId = GetUserId();
            if (userId is null)
                return Unauthorized();
            var query = new GetMyOrganizerQuery((Guid)userId);
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Отозвать свою заявку на организатора
        /// </summary>
        /// <returns>Результат операции</returns>
        //[HttpDelete("my")]
        //public async Task<IActionResult> DeleteMyOrganizerCommand()
        //{
        //    var command = new DeleteMyOrganizerCommand();
        //    var result = await _mediator.Send(command);
        //    return HandleResult(result);
        //}
    }
}