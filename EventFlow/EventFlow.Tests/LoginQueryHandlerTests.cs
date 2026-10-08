using EventFlow.Application.Queries.LoginQuery;
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
    public class LoginQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<LoginQuery>> _validatorMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly LoginQueryHandler _handler;

        public LoginQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IValidator<LoginQuery>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            
            _handler = new LoginQueryHandler(
                _userRepositoryMock.Object,
                _validatorMock.Object,
                _tokenServiceMock.Object,
                _refreshTokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            // Arrange
            var query = new LoginQuery("test@example.com", "password123");
            
            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is required"));
            
            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Email is required", result.Message);
            
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ReturnsFailureResult()
        {
            // Arrange
            var query = new LoginQuery("test@example.com", "password123");
            
            var validationResult = new FluentValidation.Results.ValidationResult();
            
            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(query.Email))
                               .ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Пользователь с таким Email и пароль не найден", result.Message);
            
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(query.Email), Times.Once);
            _userRepositoryMock.Verify(r => r.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public async Task Handle_WhenPasswordIsIncorrect_ReturnsFailureResult()
        {
            // Arrange
            var query = new LoginQuery("test@example.com", "password123");
            var user = new User 
            { 
                Id = Guid.NewGuid(), 
                Email = query.Email,
                PasswordHash = "hashed_password"
            };
            
            var validationResult = new FluentValidation.Results.ValidationResult();
            
            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(query.Email))
                               .ReturnsAsync(user);
            
            _userRepositoryMock.Setup(r => r.CheckPasswordAsync(user, query.Password))
                               .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            Assert.Contains("Пользователь с таким Email и пароль не найден", result.Message);
            
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(query.Email), Times.Once);
            _userRepositoryMock.Verify(r => r.CheckPasswordAsync(user, query.Password), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAllConditionsAreMet_ReturnsSuccessResult()
        {
            // Arrange
            var query = new LoginQuery("test@example.com", "password123");
            var user = new User 
            { 
                Id = Guid.NewGuid(), 
                Email = query.Email,
                PasswordHash = "hashed_password"
            };
            
            var validationResult = new FluentValidation.Results.ValidationResult();
            
            _validatorMock.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(query.Email))
                               .ReturnsAsync(user);
            
            _userRepositoryMock.Setup(r => r.CheckPasswordAsync(user, query.Password))
                               .ReturnsAsync(true);
            
            _tokenServiceMock.Setup(t => t.GenerateTokenAsync(user))
                            .ReturnsAsync("access_token");
            
            _refreshTokenServiceMock.Setup(r => r.GenerateAndSaveRefreshTokenAsync(user.Id))
                                   .ReturnsAsync("refresh_token");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result);
            Assert.Equal("access_token", result.Value?.accessToken);
            Assert.Equal("refresh_token", result.Value?.refreshToken);
            
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(query.Email), Times.Once);
            _userRepositoryMock.Verify(r => r.CheckPasswordAsync(user, query.Password), Times.Once);
            _tokenServiceMock.Verify(t => t.GenerateTokenAsync(user), Times.Once);
            _refreshTokenServiceMock.Verify(r => r.GenerateAndSaveRefreshTokenAsync(user.Id), Times.Once);
        }
    }
}