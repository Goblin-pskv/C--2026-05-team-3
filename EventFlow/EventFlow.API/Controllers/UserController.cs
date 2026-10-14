using System.Security.Claims;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Queries.GetProfileQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventFlow.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        public UserController(IMediator mediator) : base(mediator)
        {
        }
        /// <summary>
        /// Обновление профиля пользователя
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Возвращает сообщение с кодом ответа</returns>
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileCommand([FromBody] UpdateProfileCommand command)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized("Пользователь не авторизован");

            var commandWithGuid = command with { UserId = userId.Value };

            var result = await _mediator.Send(commandWithGuid);

            return HandleResult(result);
        }
        /// <summary>
        /// Получение данных профиля пользователя
        /// </summary>
        /// <returns>Возвращает сообщение с кодом ответа</returns>
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfileCommand()
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Пользователь не авторизован");
            var command = new GetProfileQuery(userId.Value);
            var result = await _mediator.Send(command);

            return HandleResult(result);
        }
    }
}