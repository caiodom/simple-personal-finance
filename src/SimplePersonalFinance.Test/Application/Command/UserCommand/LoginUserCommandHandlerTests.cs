using Moq;
using SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Command.UserCommands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthService>();
        _handler = new LoginUserCommandHandler(_authServiceMock.Object, _userRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldVerifyPasswordAndReturnToken()
    {
        const string email = "user@example.test";
        const string password = "test-input";
        const string storedHash = "test-hash";
        const string token = "test-token";
        var user = User.Create("Test User", email, storedHash, "client", new DateTime(1993, 3, 1)).Value;
        var command = new LoginUserCommand(email, password);

        _userRepositoryMock.Setup(repository => repository.GetByEmailAsync(email)).ReturnsAsync(user);
        _authServiceMock.Setup(service => service.VerifyPassword(password, storedHash)).Returns(true);
        _authServiceMock.Setup(service => service.GenerateJwtToken(user.Id, email, user.Role)).Returns(token);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(email, result.Data.Email);
        Assert.Equal(token, result.Data.Token);
        _authServiceMock.Verify(service => service.VerifyPassword(password, storedHash), Times.Once);
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldThrowInvalidCredentialsAndNotGenerateToken()
    {
        const string email = "user@example.test";
        const string password = "wrong-test-input";
        const string storedHash = "test-hash";
        var user = User.Create("Test User", email, storedHash, "client", new DateTime(1993, 3, 1)).Value;
        var command = new LoginUserCommand(email, password);

        _userRepositoryMock.Setup(repository => repository.GetByEmailAsync(email)).ReturnsAsync(user);
        _authServiceMock.Setup(service => service.VerifyPassword(password, storedHash)).Returns(false);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(command, CancellationToken.None));

        _authServiceMock.Verify(service => service.GenerateJwtToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithUnknownEmail_ShouldThrowInvalidCredentialsWithoutPasswordVerification()
    {
        const string email = "missing@example.test";
        var command = new LoginUserCommand(email, "test-input");

        _userRepositoryMock.Setup(repository => repository.GetByEmailAsync(email)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(command, CancellationToken.None));

        _authServiceMock.Verify(service => service.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _authServiceMock.Verify(service => service.GenerateJwtToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
