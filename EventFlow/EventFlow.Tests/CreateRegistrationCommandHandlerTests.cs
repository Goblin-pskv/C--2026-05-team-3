using EventFlow.Application.Commands.RegistrationCommands;
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
    public class CreateRegistrationCommandHandlerTests
    {
        private readonly Mock<IRegistrationRepository> _registrationRepositoryMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IValidator<CreateRegistrationCommand>> _validatorMock;
        private readonly Mock<ILogger<CreateRegistrationCommandHandle>> _loggerMock;
        private readonly CreateRegistrationCommandHandle _handler;

        public CreateRegistrationCommandHandlerTests()
        {
            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _validatorMock = new Mock<IValidator<CreateRegistrationCommand>>();
            _loggerMock = new Mock<ILogger<CreateRegistrationCommandHandle>>();
            
            _handler = new CreateRegistrationCommandHandle(
                _registrationRepositoryMock.Object,
                _eventRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("EventId", "EventId is required"));

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEventNotFound_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Event?)null);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEventNotPublished_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var @event = new Event
            {
                Id = command.EventId,
                IsPublished = false
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(422, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEventAlreadyStarted_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var @event = new Event
            {
                Id = command.EventId,
                IsPublished = true,
                Start = DateTime.UtcNow.AddHours(-1)
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(422, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserAlreadyRegistered_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                EventId = command.EventId,
                Status = RegistrationStatus.Confirmed
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new Event { Id = command.EventId, IsPublished = true, Start = DateTime.UtcNow.AddDays(1) });

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(registration);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserAlreadyCancelled_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var registration = new Registration
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                EventId = command.EventId,
                Status = RegistrationStatus.Cancelled
            };
            var @event = new Event
            {
                Id = command.EventId,
                IsPublished = true,
                Start = DateTime.UtcNow.AddDays(1)
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(registration);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.UpdateAsync(registration, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEventIsFull_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var @event = new Event
            {
                Id = command.EventId,
                IsPublished = true,
                Start = DateTime.UtcNow.AddDays(1),
                MaxParticipants = 1,
                Title = "Test Event"
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync((Registration?)null);
            
            _registrationRepositoryMock.Setup(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(422, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenAllConditionsAreMet_ReturnsSuccessResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var @event = new Event
            {
                Id = command.EventId,
                IsPublished = true,
                Start = DateTime.UtcNow.AddDays(1),
                MaxParticipants = null
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync((Registration?)null);
            
            _registrationRepositoryMock.Setup(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenDbUpdateExceptionOccurs_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var @event = new Event
            {
                Id = command.EventId,
                IsPublished = true,
                Start = DateTime.UtcNow.AddDays(1),
                MaxParticipants = null
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync((Registration?)null);
            
            _registrationRepositoryMock.Setup(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(0);
            
            _registrationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()))
                                       .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Database error"));

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_ReturnsFailureResult()
        {
            // Arrange
            var command = new CreateRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());
            var @event = new Event
            {
                Id = command.EventId,
                IsPublished = true,
                Start = DateTime.UtcNow.AddDays(1),
                MaxParticipants = null
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync((Registration?)null);
            
            _registrationRepositoryMock.Setup(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(0);
            
            _registrationRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()))
                                       .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(500, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationAsync(command.UserId, command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.CountConfirmedAsync(command.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Registration>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}