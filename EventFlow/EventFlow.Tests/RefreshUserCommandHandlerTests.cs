using EventFlow.Application.Commands.AuthCommands;
using EventFlow.Application.Interfaces;
using EventFlow.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventFlow.Tests
{
    public class RefreshUserCommandHandlerTests
    {
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<ITokenHasher> _tokenHasherMock;
        private readonly Mock<ILogger<RefreshUserCommand>> _loggerMock;
        private readonly RefreshUserCommandHandler _handler;
        public RefreshUserCommandHandlerTests()
        {
            _tokenServiceMock = new Mock<ITokenService>();
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _tokenHasherMock = new Mock<ITokenHasher>();
            _loggerMock = new Mock<ILogger<RefreshUserCommand>>();
            _handler = new RefreshUserCommandHandler(
                _tokenServiceMock.Object,
                _refreshTokenServiceMock.Object,
                _tokenHasherMock.Object,
                _loggerMock.Object);
        }
        [Fact]
        public async Task Handle_WhenRefreshFound_ReturnsSuccess()
        {
            //Arrange
            var command = new RefreshUserCommand("kme;wlrmop234j;lwedmrf;l12@!ewq;r!2");
            var user = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "feqef"
            };
            _tokenHasherMock.Setup(th => th.Hash(command.refresh))
                .Returns(";kgm;sfdakgmlsdkfgmsd;lkfgs");
            _refreshTokenServiceMock.Setup(rts => rts.ValidateRefreshTokenAsync(";kgm;sfdakgmlsdkfgmsd;lkfgs"))
                .ReturnsAsync(user);
            //Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());
            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            _tokenServiceMock.Verify(rts => rts.GenerateTokenAsync(It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public async Task Handle_WhenUserIsNull_ReturnsFailureResult()
        {
            //Arrange
            var command = new RefreshUserCommand("kme;wlrmop234j;lwedmrf;l12@!ewq;r!2");
            _tokenHasherMock.Setup(th => th.Hash(command.refresh))
                .Returns(";kgm;sfdakgmlsdkfgmsd;lkfgs");
            _refreshTokenServiceMock.Setup(rts => rts.ValidateRefreshTokenAsync("htsfghtesfhgfkls;dafadfasd"))
                .ReturnsAsync((User?)null);
            //Act
            var result = await _handler.Handle(command, It.IsAny<CancellationToken>());
            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(401, result.StatusCode);
            _tokenServiceMock.Verify(rts => rts.GenerateTokenAsync(It.IsAny<User>()), Times.Never);
        }
    }
}