using EventFlow.Application.Commands.UpdateProfileCommand;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventFlow.Tests
{
    public class UpdateProfileCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IValidator<UpdateProfileCommand>> _validatorMock;
        private readonly Mock<ILogger<UpdateProfileCommandHandler>> _loggerMock;
        private readonly UpdateProfileCommandHandler _handler;

        public UpdateProfileCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validatorMock = new Mock<IValidator<UpdateProfileCommand>>();
            _loggerMock = new Mock<ILogger<UpdateProfileCommandHandler>>();
            
            _handler = new UpdateProfileCommandHandler(
                _userRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ReturnsFailureResult()
        {
            var command = new UpdateProfileCommand(
                Guid.NewGuid(),
                "Иван",
                "Иванов",
                "ivan@example.com",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", "Email is required"));

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.False(result.IsSuccess);
            Assert.Equal(422, result.StatusCode);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _userRepositoryMock.Verify(r => r.ExistsByEmailAsync(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ReturnsFailureResult()
        {
            var userId = Guid.NewGuid();
            var command = new UpdateProfileCommand(
                userId,
                "Иван",
                "Иванов",
                "ivan@example.com",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                              .ReturnsAsync((User?)null);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.False(result.IsSuccess);
            Assert.Equal(404, result.StatusCode);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExists_ReturnsFailureResult()
        {
            var userId = Guid.NewGuid();
            var command = new UpdateProfileCommand(
                userId,
                "Иван",
                "Иванов",
                "ivan@example.com",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            var user = new User
            {
                Id = userId,
                FirstName = "Петр",
                LastName = "Петров",
                Email = "petr@example.com"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                              .ReturnsAsync(user);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(true);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.False(result.IsSuccess);
            Assert.Equal(409, result.StatusCode);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.ExistsByEmailAsync(command.Email), Times.Once);
        }
        [Fact]
        public async Task Handle_WhenUpdateFails_ReturnsFailureResult()
        {
            var userId = Guid.NewGuid();
            var command = new UpdateProfileCommand(
                userId,
                "Иван",
                "Иванов",
                "ivan@example.com",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            var user = new User
            {
                Id = userId,
                FirstName = "Петр",
                LastName = "Петров",
                Email = "petr@example.com"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                              .ReturnsAsync(user);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(false);

            var identityResult = Microsoft.AspNetCore.Identity.IdentityResult.Failed(
                new Microsoft.AspNetCore.Identity.IdentityError { Code = "Error1", Description = "Something went wrong" }
            );

            _userRepositoryMock.Setup(r => r.Update(user))
                              .ReturnsAsync(identityResult);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.False(result.IsSuccess);
            Assert.Equal(400, result.StatusCode);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.ExistsByEmailAsync(command.Email), Times.Once);
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAllConditionsAreMet_ReturnsSuccessResult()
        {
            var userId = Guid.NewGuid();
            var command = new UpdateProfileCommand(
                userId,
                "Иван",
                "Иванов",
                "ivan@example.com",
                "1234567890"
            );

            var validationResult = new FluentValidation.Results.ValidationResult();

            _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(validationResult);

            var user = new User
            {
                Id = userId,
                FirstName = "Петр",
                LastName = "Петров",
                Email = "petr@example.com"
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId))
                              .ReturnsAsync(user);
            
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(command.Email))
                              .ReturnsAsync(false);

            var identityResult = Microsoft.AspNetCore.Identity.IdentityResult.Success;

            _userRepositoryMock.Setup(r => r.Update(user))
                              .ReturnsAsync(identityResult);

            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());

            Assert.True(result.IsSuccess);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.ExistsByEmailAsync(command.Email), Times.Once);
            _userRepositoryMock.Verify(r => r.Update(user), Times.Once);
        }
    }
}