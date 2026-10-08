using EventFlow.Application.Queries.EventQueries;
using EventFlow.Application.Common;
using EventFlow.Application.DTOs;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EventFlow.Domain.Enums;

namespace EventFlow.Tests
{
    public class GetEventQueryHandlerTests
    {
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<ILogger<GetEventQueryHandle>> _loggerMock;
        private readonly GetEventQueryHandle _handler;

        public GetEventQueryHandlerTests()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _loggerMock = new Mock<ILogger<GetEventQueryHandle>>();
            
            _handler = new GetEventQueryHandle(
                _eventRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenEventIdIsEmpty_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetEventQuery() { EventId = Guid.Empty};

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("EventId обязателен", result.Message);
            _eventRepositoryMock.Verify(r => r.GetByIdWithIncludesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenEventNotFound_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetEventQuery() { EventId = Guid.NewGuid() };

            _eventRepositoryMock.Setup(r => r.GetByIdWithIncludesAsync(query.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync((Event?)null);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains($"Мероприятие с Id {query.EventId} не найдено", result.Message);
            _eventRepositoryMock.Verify(r => r.GetByIdWithIncludesAsync(query.EventId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEventFound_ReturnsSuccessResult()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var query = new GetEventQuery() { EventId = Guid.NewGuid() };
            
            var eventEntity = new Event
            {
                Id = eventId,
                Title = "Test Event",
                Description = "Test Description",
                Type = EventType.Conference,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2),
                City = "Moscow",
                Address = "Test Address",
                Price = 1000m,
                MaxParticipants = 100,
                OrganizerId = Guid.NewGuid(),
                IsPublished = true,
                CreatedAt = DateTime.UtcNow
            };

            _eventRepositoryMock.Setup(r => r.GetByIdWithIncludesAsync(query.EventId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(eventEntity);

            // Act
            var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(eventEntity.Title, result.Value.Title);
            Assert.Equal(eventEntity.Description, result.Value.Description);
            Assert.Equal(eventEntity.Type.ToString(), result.Value.Type);
            Assert.Equal(eventEntity.Start, result.Value.Start);
            Assert.Equal(eventEntity.End, result.Value.End);
            Assert.Equal(eventEntity.City, result.Value.City);
            Assert.Equal(eventEntity.Address, result.Value.Address);
            Assert.Equal(eventEntity.Price, result.Value.Price);
            Assert.Equal(eventEntity.MaxParticipants, result.Value.MaxParticipants);
            Assert.Equal(eventEntity.IsPublished, result.Value.IsPublished);
            Assert.Equal(eventEntity.CreatedAt, result.Value.CreatedAt);
            _eventRepositoryMock.Verify(r => r.GetByIdWithIncludesAsync(query.EventId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}