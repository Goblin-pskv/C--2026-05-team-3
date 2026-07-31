using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Commands.UpdateProfileCommand;
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
        public TestController(RegisterCommandHandler registerHandler, UpdateProfileCommandHandler updateHandler)
        {
            _registerHandler = registerHandler;
            _updateHandler = updateHandler;
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterUserCommand command, CancellationToken ct)
        {
            command = new RegisterUserCommand("test", "test2", "awsd@e.ru", "asfddasfsdf", "dsad");
            await _registerHandler.Handle(command, ct);
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateProfileCommand command, CancellationToken ct)
        {
            try
            {
                command = new UpdateProfileCommand(Guid.Parse(id),"test2","test3","test@t.com","qwedasdewq");
                await _updateHandler.Handle(command, ct);
                return Ok(new { success = true, message = "Данные обновлены" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
