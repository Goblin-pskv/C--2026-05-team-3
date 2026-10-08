using EventFlow.Application.Queries.GetProfileQuery;
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

namespace EventFlow.Tests
{
    public class GetProfileQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetProfileQueryHandler _handler;

        public GetProfileQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            
            _handler = new GetProfileQueryHandler(
                _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetProfileQuery(Guid.NewGuid());
            
            _userRepositoryMock.Setup(r => r.GetByIdAsync(query.UserId))
                               .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            Assert.Contains("Пользователь не найден", result.Message);
            
            _userRepositoryMock.Verify(r => r.GetByIdAsync(query.UserId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserFound_ReturnsSuccessResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetProfileQuery(userId);
            var user = new User 
            { 
                Id = userId,
                FirstName = "Дима",
                LastName = "В.",
                Email = "v.d@example.com",
                PhoneNumber = "1234567890"
            };
            
            _userRepositoryMock.Setup(r => r.GetByIdAsync(query.UserId))
                               .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal("Дима", result.Value.FirstName);
            Assert.Equal("В.", result.Value.LastName);
            Assert.Equal("v.d@example.com", result.Value.Email);
            Assert.Equal("1234567890", result.Value.PhoneNumber);
            
            _userRepositoryMock.Verify(r => r.GetByIdAsync(query.UserId), Times.Once);
        }
    }
}