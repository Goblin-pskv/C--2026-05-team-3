using EventFlow.Application.Queries.RegistrationQueries;
using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventFlow.Tests
{
    public class GetEventRegistrationsQueryHandlerTests
    {
        private readonly Mock<IRegistrationRepository> _registrationRepositoryMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IValidator<GetEventRegistrationsQuery>> _validatorMock;
        private readonly Mock<ILogger<GetEventRegistrationsQueryHandle>> _loggerMock;
        private readonly GetEventRegistrationsQueryHandle _handler;

        public GetEventRegistrationsQueryHandlerTests()
        {
            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _validatorMock = new Mock<IValidator<GetEventRegistrationsQuery>>();
            _loggerMock = new Mock<ILogger<GetEventRegistrationsQueryHandle>>();

            _handler = new GetEventRegistrationsQueryHandle(
                _registrationRepositoryMock.Object,
                _eventRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetEventRegistrationsQuery(Guid.NewGuid());

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("EventId", "EventId is required"));

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _eventRepositoryMock.Verify(r => r.GetEventWithRegistrationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEventNotFound_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetEventRegistrationsQuery(Guid.NewGuid());

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(query.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Event?)null);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains($"Мероприятие с Id {query.EventId} не найдено", result.Message);
            _eventRepositoryMock.Verify(r => r.GetByIdAsync(query.EventId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenRegistrationsFound_ReturnsSuccessResult()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var query = new GetEventRegistrationsQuery(eventId);

            var @event = new Event
            {
                Id = eventId,
                Title = "Test Event",
                Start = DateTime.UtcNow.AddDays(1)
            };
            var user = new User() { Id = Guid.NewGuid(), FirstName = "Дима", LastName = "В.", Email = "v.dima@example.com"};
            var registrations = new List<Registration>
            {
                new Registration
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    UserId = user.Id,
                    Status = Domain.Enums.RegistrationStatus.Confirmed,
                    RegistrationDate = DateTime.UtcNow.AddDays(-1),
                    ConfirmationDate = DateTime.UtcNow,
                    User = user
                }
            };
            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _eventRepositoryMock.Setup(r => r.GetByIdAsync(query.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(@event);

            _registrationRepositoryMock.Setup(r => r.GetRegistrationsByEventIdAsync(query.EventId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(registrations);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);

            Assert.Equal(registrations[0].Id, result.Value[0].Id);
            Assert.Equal(registrations[0].EventId, result.Value[0].EventId);
            Assert.Equal(@event.Title, result.Value[0].EventTitle);
            Assert.Equal(@event.Start, result.Value[0].EventStart);
            Assert.Equal(registrations[0].UserId, result.Value[0].UserId);
            Assert.Equal("Дима В.", result.Value[0].UserName);
            Assert.Equal("v.dima@example.com", result.Value[0].UserEmail);
            Assert.Equal(registrations[0].Status.ToString(), result.Value[0].Status);
            Assert.Equal(registrations[0].RegistrationDate, result.Value[0].RegistrationDate);
            Assert.Equal(registrations[0].ConfirmationDate, result.Value[0].ConfirmationDate);

            _eventRepositoryMock.Verify(r => r.GetByIdAsync(query.EventId, It.IsAny<CancellationToken>()), Times.Once);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationsByEventIdAsync(query.EventId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}