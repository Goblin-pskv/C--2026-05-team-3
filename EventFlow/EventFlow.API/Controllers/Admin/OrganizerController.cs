using EventFlow.Application.Commands.OrganizerCommands;
using EventFlow.Application.Common;
using EventFlow.Application.Queries.OrganizerQueries;
using EventFlow.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EventFlow.API.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class OrganizerController : BaseController
    {
        public OrganizerController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Получить список заявок организаторов
        /// </summary>
        /// <returns>Список заявок</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllOrganizerCommand([FromQuery] GetAllOrganizerQuery command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Получить детали заявки организатора
        /// </summary>
        /// <param name="id">ID заявки</param>
        /// <returns>Данные заявки</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganizerByIdCommand(Guid id)
        {
            var query = new GetOrganizerByIdQuery(id);
            var result = await _mediator.Send(query);
            return HandleResult(result);
        }

        /// <summary>
        /// Одобрить заявку организатора
        /// </summary>
        /// <param name="id">ID заявки</param>
        /// <returns>Результат операции</returns>
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> ApproveOrganizerCommand(Guid id)
        {
            var command = new ApproveOrganizerCommand(id);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Отклонить заявку организатора
        /// </summary>
        /// <param name="id">ID заявки</param>
        /// <param name="reason">Причина отклонения</param>
        /// <returns>Результат операции</returns>
        [HttpDelete("{id}/reject")]
        public async Task<IActionResult> RejectOrganizerCommand(Guid id, [FromBody] string reason)
        {
            var command = new RejectOrganizerCommand (id, reason);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}