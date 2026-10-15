using EventFlow.Application.Commands.RegisterCommand;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using EventFlow.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
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
        private readonly Mock<ILogger<RegisterUserCommandHandler>> _loggerMock;
        private readonly Mock<IEmailConfirmTokenService> _emailConfirmTokenService;
        private readonly Mock<IMessageService> _messageServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<RegisterUserCommand>>();
            _loggerMock = new Mock<ILogger<RegisterUserCommandHandler>>();
            _emailConfirmTokenService = new Mock<IEmailConfirmTokenService>();
            _messageServiceMock = new Mock<IMessageService>();
            
            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWork.Object,
                _validatorMock.Object,
                _loggerMock.Object,
                _emailConfirmTokenService.Object,
                _messageServiceMock.Object);
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
                "1234567890",
                "url"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is required"));

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(422, result.StatusCode);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), command.Password), Times.Never);
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
                "1234567890",
                "url"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            // Asserts
            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.StatusCode);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), command.Password), Times.Never);
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
                "1234567890",
                "url"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(false);
            
            var identityResult = IdentityResult.Failed(
                new IdentityError { Code = "Error1", Description = "Something went wrong" });
            
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), command.Password))
                              .ReturnsAsync(identityResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), command.Password), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAllConditionsAreMet_ReturnsSuccessResult()
        {
            // Arrange
            var command = new RegisterUserCommand(
                "testuser",
                "Test",
                "User",
                "admin@eventflow.com",
                "Password123!",
                "1234567890",
                "url"
            );
            var user = new User()
            {
                UserName = command.UserName,
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                PhoneNumber = command.PhoneNumber
            };
            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(false);
            var identityResult = IdentityResult.Success;
            _userRepositoryMock.Setup(r => r.AddAsync(It.Is<User>(u => u.Email == command.Email && u.UserName == command.UserName)
                , command.Password))
                .ReturnsAsync(identityResult);
            _emailConfirmTokenService.Setup(r => r.GenerateAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync("");
            _messageServiceMock.Setup(r => r.SendEmailAsync(user.Email, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()));
            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), command.Password), Times.Once);
            _emailConfirmTokenService.Verify(r => r.GenerateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _messageServiceMock.Verify(r => r.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),Times.Once);
        }
    }
}