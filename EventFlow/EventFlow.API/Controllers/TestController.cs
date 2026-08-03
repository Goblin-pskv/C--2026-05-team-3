using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Queries.GetProfileQuery;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly RegisterCommandHandler _registerHandler;
        private readonly UpdateProfileCommandHandler _updateHandler;
        private readonly GetProfileQueryHandler _getProfileQueryHandler;

        public TestController(RegisterCommandHandler registerHandler, UpdateProfileCommandHandler updateHandler, GetProfileQueryHandler getProfileQuery)
        {
            _registerHandler = registerHandler;
            _updateHandler = updateHandler;
            _getProfileQueryHandler = getProfileQuery;
        }

        // POST api/<ValuesController>
        [HttpPost("register")]
        public async Task<IActionResult> Registration([FromBody] RegisterUserCommand command, CancellationToken ct)
        {
            command = new RegisterUserCommand("UserNameNew1", "test", "test2", "awsd@e.ru", "asfdda1D@", "dsad");
            var result = await _registerHandler.Handle(command, ct);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok();
        }
        [HttpPut("UpdateUser{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateProfileCommand command, CancellationToken ct)
        {
            try
            {
                command = new UpdateProfileCommand(Guid.Parse(id),"test2","test3","te@t.com","qwedasdewq");
                await _updateHandler.Handle(command, ct);
                return Ok(new { success = true, message = "Данные обновлены" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPut("GetProfile")]
        public async Task<IActionResult> GetProfile([FromQuery] GetProfileQuery getProfileQuery, CancellationToken ct)
        {
            var dto = await _getProfileQueryHandler.Handle(getProfileQuery, ct);
            return Ok(dto);
        }
    }
}
