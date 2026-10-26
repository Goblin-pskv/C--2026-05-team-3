using EventFlow.Application.Commands.PaymentCommands;
using EventFlow.Application.Queries.PaymentQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : BaseController
    {
        public PaymentsController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Начать оплату билета
        /// </summary>
        /// <param name="ticketId">ID билета</param>
        /// <returns>Данные платежа</returns>
        [HttpPost("initiate/{ticketId:guid}")]
        public async Task<IActionResult> InitiatePaymentCommand(Guid ticketId)
        {
            Guid? userId = GetUserId();
            if (userId is null)
                return Unauthorized();
                
            var command = new InitiatePaymentCommand(ticketId, (Guid)userId);
            var result = await _mediator.Send(command);

            return HandleResult(result);
        }

        /// <summary>
        /// Подтвердить оплату
        /// </summary>
        /// <param name="id">ID платежа</param>
        /// <returns>Результат операции</returns>
        [HttpPost("{id:guid}/confirm")]
        public async Task<IActionResult> ConfirmPaymentCommand(Guid id)
        {
            Guid? userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var command = new ConfirmPaymentCommand(id, (Guid)userId);
            var result = await _mediator.Send(command);

            return HandleResult(result);
        }

        /// <summary>
        /// Получить статус платежа
        /// </summary>
        /// <param name="id">ID платежа</param>
        /// <returns>Данные платежа</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> PaymentStatusQuery(Guid id)
        {
            Guid? userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var query = new PaymentStatusQuery(id, (Guid)userId);
            var result = await _mediator.Send(query);

            return HandleResult(result);
        }

        /// <summary>
        /// Получить историю своих платежей
        /// </summary>
        /// <returns>Список платежей</returns>
        [HttpGet("my")]
        public async Task<IActionResult> PaymentsQuery()
        {
            Guid? userId = GetUserId();
            if (userId is null)
                return Unauthorized();

            var query = new PaymentsQuery((Guid)userId);
            var result = await _mediator.Send(query);

            return HandleResult(result);
        }
    }
}
