using EventFlow.Application.Commands.EventCommands;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;

namespace EventFlow.Tests
{
    public class CreateEventCommandHandlerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IValidator<CreateEventCommand>> _validatorMock;
        private readonly Mock<ILogger<CreateEventCommandHandle>> _loggerMock;
        private readonly CreateEventCommandHandle _handler;

        public CreateEventCommandHandlerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _validatorMock = new Mock<IValidator<CreateEventCommand>>();
            _loggerMock = new Mock<ILogger<CreateEventCommandHandle>>();
            
            _handler = new CreateEventCommandHandle(
                _eventRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenAllConditionsAreMet_ReturnsSuccessResult()
        {
            var command = new CreateEventCommand(
                "Test Event",
                "Test Description",
                Domain.Enums.EventType.Conference,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                "Moscow",
                "Test Address",
                1000m,
                100,
                Guid.NewGuid(),
                false
                );
            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
            _eventRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            var command = new CreateEventCommand(
                "Test Event",
                "Test Description",
                Domain.Enums.EventType.Conference,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                "Moscow",
                "Test Address",
                1000m,
                100,
                Guid.NewGuid(),
                false
            );

            var validationResult = new ValidationResult();
            validationResult.Errors.Add(new ValidationFailure("Title", "Title is required"));

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.False(result.IsSuccess);
            Assert.Equal(422, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenSaveAsyncFails_ReturnsFailureResult()
        {
            var command = new CreateEventCommand(
                "Test Event",
                "Test Description",
                Domain.Enums.EventType.Conference,
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(2),
                "Moscow",
                "Test Address",
                1000m,
                100,
                Guid.NewGuid(),
                false
            );

            var validationResult = new ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new DbUpdateException("Database error"));

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.False(result.IsSuccess);
            Assert.Equal(500, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once);
            _eventRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}