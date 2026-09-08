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
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Обновление профиля пользователя
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Возвращает сообщение с кодом ответа</returns>
        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfileCommand([FromBody] UpdateProfileCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Пользователь не авторизован");

            var commandWithGuid = command with { UserId = Guid.Parse(userId) };

            var result = await _mediator.Send(commandWithGuid);

            if (!result.IsSuccess)
            {
                switch (result.StatusCode)
                {
                    case 400:
                        return BadRequest(result.Message);
                    case 409:
                        return Conflict(result.Message);
                    case 422:
                        return UnprocessableEntity(result.Message);
                }
            }
            return Ok(result.Message);
        }
        /// <summary>
        /// Получение данных профиля пользователя
        /// </summary>
        /// <returns>Возвращает сообщение с кодом ответа</returns>
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfileCommand()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Пользователь не авторизован");
            var command = new GetProfileQuery(Guid.Parse(userId));
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
    }
}
