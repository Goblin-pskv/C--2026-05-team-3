using EventFlow.Application.Commands.AuthCommands;
using EventFlow.Application.Interfaces;
using EventFlow.Application.Common;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventFlow.Tests
{
    public class ConfirmEmailCommandHandlerTests
    {
        private readonly Mock<IEmailConfirmTokenService> _emailConfirmTokenServiceMock;
        private readonly Mock<ILogger<ConfirmEmailCommandHandler>> _loggerMock;
        private readonly ConfirmEmailCommandHandler _handler;

        public ConfirmEmailCommandHandlerTests()
        {
            _emailConfirmTokenServiceMock = new Mock<IEmailConfirmTokenService>();
            _loggerMock = new Mock<ILogger<ConfirmEmailCommandHandler>>();
            
            _handler = new ConfirmEmailCommandHandler(
                _emailConfirmTokenServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEmailConfirmationFails_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "fake-token";
            var command = new ConfirmEmailCommand(userId, token);

            _emailConfirmTokenServiceMock.Setup(s => s.ConfirmEmail(userId, token, It.IsAny<CancellationToken>()))
                                         .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Неверная ссылка подтверждения или срок действия токена истек", result.Message);
        }

        [Fact]
        public async Task Handle_WhenEmailConfirmationSucceeds_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "fake-token";
            var command = new ConfirmEmailCommand(userId, token);

            _emailConfirmTokenServiceMock.Setup(s => s.ConfirmEmail(userId, token, It.IsAny<CancellationToken>()))
                                         .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Email успешно подтвержден", result.Message);
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_ReturnsFailureResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "fake-token";
            var command = new ConfirmEmailCommand(userId, token);

            _emailConfirmTokenServiceMock.Setup(s => s.ConfirmEmail(userId, token, It.IsAny<CancellationToken>()))
                                         .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(500, result.StatusCode);
            Assert.Equal("Произошла ошибка при подтверждении email", result.Message);
        }
    }
}