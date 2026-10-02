using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventFlow.Tests
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<RegisterUserCommand>> _validatorMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IValidator<RegisterUserCommand>>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();
            
            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _validatorMock.Object,
                _tokenServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            // Arrange
            var command = new RegisterUserCommand(
                "testuser",
                "Test",
                "User",
                "test@example.com",
                "Password123!",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is required"));

            _validatorMock.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(422, result.StatusCode);
        }

        [Fact]
        public async Task Handle_WhenUserAlreadyExists_ReturnsFailureResult()
        {
            // Arrange
            var command = new RegisterUserCommand(
                "testuser",
                "Test",
                "User",
                "wq@qwamse.ru",
                "Password123!",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.StatusCode);
        }

        [Fact]
        public async Task Handle_WhenAddAsyncFails_ReturnsFailureResult()
        {
            // Arrange
            var command = new RegisterUserCommand(
                "testuser",
                "Test",
                "User",
                "test@example.com",
                "Password123!",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(false);
            
            var identityResult = Microsoft.AspNetCore.Identity.IdentityResult.Failed(
                new Microsoft.AspNetCore.Identity.IdentityError { Code = "Error1", Description = "Something went wrong" }
            );
            
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), command.Password))
                              .ReturnsAsync(identityResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task Handle_WhenAllConditionsAreMet_ReturnsSuccessResult()
        {
            // Arrange
            var command = new RegisterUserCommand(
                "testuser",
                "Test",
                "User",
                "test@example.com",
                "Password123!",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(false);
            
            var identityResult = Microsoft.AspNetCore.Identity.IdentityResult.Success;
            
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), command.Password))
                              .ReturnsAsync(identityResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}