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
    public class GetUserRegistrationsQueryHandlerTests
    {
        private readonly Mock<IRegistrationRepository> _registrationRepositoryMock;
        private readonly Mock<IValidator<GetUserRegistrationsQuery>> _validatorMock;
        private readonly Mock<ILogger<GetUserRegistrationsQueryHandle>> _loggerMock;
        private readonly GetUserRegistrationsQueryHandle _handler;

        public GetUserRegistrationsQueryHandlerTests()
        {
            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _validatorMock = new Mock<IValidator<GetUserRegistrationsQuery>>();
            _loggerMock = new Mock<ILogger<GetUserRegistrationsQueryHandle>>();
            
            _handler = new GetUserRegistrationsQueryHandle(
                _registrationRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetUserRegistrationsQuery(Guid.NewGuid());
            
            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("UserId", "UserId is required"));

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            _registrationRepositoryMock.Verify(r => r.GetRegistrationsByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRegistrationsFound_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserRegistrationsQuery(userId);
            
            var registrations = new List<Registration>
            {
                new Registration
                {
                    Id = Guid.NewGuid(),
                    EventId = Guid.NewGuid(),
                    UserId = userId,
                    Status = Domain.Enums.RegistrationStatus.Confirmed,
                    RegistrationDate = DateTime.UtcNow.AddDays(-1),
                    ConfirmationDate = DateTime.UtcNow,
                    Event = new Event
                    {
                        Id = Guid.NewGuid(),
                        Title = "Test Event 1",
                        Start = DateTime.UtcNow.AddDays(1)
                    },
                    User = new User
                    {
                        Id = userId,
                        FirstName = "Дима",
                        LastName = "В.",
                        Email = "v.d@example.com"
                    }
                },
                new Registration
                {
                    Id = Guid.NewGuid(),
                    EventId = Guid.NewGuid(),
                    UserId = userId,
                    Status = Domain.Enums.RegistrationStatus.Confirmed,
                    RegistrationDate = DateTime.UtcNow.AddDays(-2),
                    ConfirmationDate = DateTime.UtcNow.AddDays(-1),
                    Event = new Event
                    {
                        Id = Guid.NewGuid(),
                        Title = "Test Event 2",
                        Start = DateTime.UtcNow.AddDays(2)
                    },
                    User = new User
                    {
                        Id = userId,
                        FirstName = "Вова",
                        LastName = "Ф.",
                        Email = "f.v@example.com"
                    }
                }
            };

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            
            _registrationRepositoryMock.Setup(r => r.GetRegistrationsByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(registrations);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);
            
            // Проверим данные первого участника
            Assert.Equal(registrations[0].Id, result.Value[0].Id);
            Assert.Equal(registrations[0].EventId, result.Value[0].EventId);
            Assert.Equal(registrations[0].Event.Title, result.Value[0].EventTitle);
            Assert.Equal(registrations[0].Event.Start, result.Value[0].EventStart);
            Assert.Equal(registrations[0].UserId, result.Value[0].UserId);
            Assert.Equal("Дима В.", result.Value[0].UserName);
            Assert.Equal("v.d@example.com", result.Value[0].UserEmail);
            Assert.Equal(registrations[0].Status.ToString(), result.Value[0].Status);
            Assert.Equal(registrations[0].RegistrationDate, result.Value[0].RegistrationDate);
            Assert.Equal(registrations[0].ConfirmationDate, result.Value[0].ConfirmationDate);

            // Проверим данные второго участника
            Assert.Equal(registrations[1].Id, result.Value[1].Id);
            Assert.Equal(registrations[1].EventId, result.Value[1].EventId);
            Assert.Equal(registrations[1].Event.Title, result.Value[1].EventTitle);
            Assert.Equal(registrations[1].Event.Start, result.Value[1].EventStart);
            Assert.Equal(registrations[1].UserId, result.Value[1].UserId);
            Assert.Equal("Вова Ф.", result.Value[1].UserName);
            Assert.Equal("f.v@example.com", result.Value[1].UserEmail);
            Assert.Equal(registrations[1].Status.ToString(), result.Value[1].Status);
            Assert.Equal(registrations[1].RegistrationDate, result.Value[1].RegistrationDate);
            Assert.Equal(registrations[1].ConfirmationDate, result.Value[1].ConfirmationDate);
            
            _registrationRepositoryMock.Verify(r => r.GetRegistrationsByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenNoRegistrationsFound_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserRegistrationsQuery(userId);
            
            var registrations = new List<Registration>();

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            
            _registrationRepositoryMock.Setup(r => r.GetRegistrationsByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()))
                                       .ReturnsAsync(registrations);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
            
            _registrationRepositoryMock.Verify(r => r.GetRegistrationsByUserIdAsync(query.UserId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}