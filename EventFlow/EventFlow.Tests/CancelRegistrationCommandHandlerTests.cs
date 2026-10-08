using EventFlow.Application.Commands.RegistrationCommands;
using EventFlow.Application.Common;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventFlow.Tests
{
    public class CancelRegistrationCommandHandlerTests
    {
        private readonly Mock<IRegistrationRepository> _registrationRepositoryMock;
        private readonly Mock<IValidator<CancelRegistrationCommand>> _validatorMock;
        private readonly Mock<ILogger<CancelRegistrationCommandHandleMock>> _loggerMock;
        private readonly CancelRegistrationCommandHandleMock _handler;

        public CancelRegistrationCommandHandlerTests()
        {
            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _validatorMock = new Mock<IValidator<CancelRegistrationCommand>>();
            _loggerMock = new Mock<ILogger<CancelRegistrationCommandHandleMock>>();

            _handler = new CancelRegistrationCommandHandleMock(
                _registrationRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            // Arrange
            var command = new CancelRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("EventId", "EventId is required"));

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            _registrationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRegistrationNotFound_ReturnsFailureResult()
        {
            // Arrange
            var command = new CancelRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync((Registration?)null);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            _registrationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRegistrationAlreadyCancelled_ReturnsFailureResult()
        {
            // Arrange
            var command = new CancelRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                EventId = command.EventId,
                Status = RegistrationStatus.Cancelled
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(registration);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.StatusCode);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.UpdateAsync(registration, It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}