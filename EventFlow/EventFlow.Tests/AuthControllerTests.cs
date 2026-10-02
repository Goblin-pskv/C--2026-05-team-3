using System;
using System.Threading.Tasks;
using EventFlow.API.Controllers;
using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.DTOs;
using EventFlow.Application.Queries.LoginQuery;
using EventFlow.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EventFlow.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthController(_mediatorMock.Object);
        }

        [Fact]
        public async Task RegisterUserCommand_Returns_Ok_When_Success()
        {
            var command = new RegisterUserCommand("username", "firstName", "lastName", "email@example.com", "password", "123456789");
            var expectedResult = Result.Success(200, "Пользователь успешно зарегистрирован");

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.RegisterUserCommand(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Пользователь успешно зарегистрирован", okResult.Value);
        }

        [Fact]
        public async Task RegisterUserCommand_Returns_BadRequest_When_400()
        {
            var command = new RegisterUserCommand("username", "firstName", "lastName", "email@example.com", "password", "123456789");
            var expectedResult = Result.Failure("Неверные данные пользователя", 400);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.RegisterUserCommand(command);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Неверные данные пользователя", badRequestResult.Value);
        }

        [Fact]
        public async Task RegisterUserCommand_Returns_Conflict_When_409()
        {
            var command = new RegisterUserCommand("username", "firstName", "lastName", "email@example.com", "password", "123456789");
            var expectedResult = Result.Failure("Пользователь с таким email уже существует", 409);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.RegisterUserCommand(command);

            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Пользователь с таким email уже существует", conflictResult.Value);
        }

        [Fact]
        public async Task RegisterUserCommand_Returns_UnprocessableEntity_When_422()
        {
            var command = new RegisterUserCommand("username", "firstName", "lastName", "email@example.com", "password", "123456789");
            var expectedResult = Result.Failure("Некорректные данные для регистрации", 422);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.RegisterUserCommand(command);

            var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal("Некорректные данные для регистрации", unprocessableEntityResult.Value);
        }
        
        [Fact]
        public async Task UserLoginCommand_Returns_Ok_When_Success()
        {
            var command = new LoginQuery("test@example.com", "password123");
            var authResponse = new AuthResponseDto("accessToken123", "refreshToken123", DateTime.Now.AddHours(1));
            var expectedResult = Result<AuthResponseDto>.Success(authResponse, 200, "Успешная авторизация");

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.UserLoginCommand(command);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(authResponse, okResult.Value);
        }

        [Fact]
        public async Task UserLoginCommand_Returns_BadRequest_When_400()
        {
            var command = new LoginQuery("test@example.com", "password123");
            var expectedResult = Result<AuthResponseDto>.Failure("Неверные учетные данные", 400);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.UserLoginCommand(command);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Неверные учетные данные", badRequestResult.Value);
        }

        [Fact]
        public async Task UserLoginCommand_Returns_NotFound_When_404()
        {
            var command = new LoginQuery("test@example.com", "password123");
            var expectedResult = Result<AuthResponseDto>.Failure("Пользователь не найден", 404);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.UserLoginCommand(command);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Пользователь не найден", notFoundResult.Value);
        }

        [Fact]
        public async Task UserLoginCommand_Returns_UnprocessableEntity_When_422()
        {
            var command = new LoginQuery("test@example.com", "password123");
            var expectedResult = Result<AuthResponseDto>.Failure("Некорректные данные авторизации", 422);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<System.Threading.CancellationToken>()))
                        .ReturnsAsync(expectedResult);

            var result = await _controller.UserLoginCommand(command);

            var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal("Некорректные данные авторизации", unprocessableEntityResult.Value);
        }
    }
}