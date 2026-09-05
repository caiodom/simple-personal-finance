using Moq;
using SimplePersonalFinance.Application.Commands.UserCommands.LoginUser;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Command.UserCommands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _authServiceMock = new Mock<IAuthService>();
        _unitOfWorkMock.Setup(x => x.Users).Returns(_userRepositoryMock.Object);
        _handler = new LoginUserCommandHandler(_authServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidPassword_ShouldReturnTokenWithoutRehash()
    {
        var command = new LoginUserCommand("user@example.com", "Password123!");
        var user = CreateUser("current-password-hash");

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash)).Returns(true);
        _authServiceMock.Setup(x => x.NeedsRehash(user.PasswordHash)).Returns(false);
        _authServiceMock.Setup(x => x.GenerateJwtToken(user.Id, user.Email.Value, user.Role)).Returns("token");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _authServiceMock.Verify(x => x.GenerateJwtToken(user.Id, user.Email.Value, user.Role), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldRejectCredentials()
    {
        var command = new LoginUserCommand("user@example.com", "WrongPassword");
        var user = CreateUser("current-password-hash");

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash)).Returns(false);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _handler.Handle(command, CancellationToken.None));

        _authServiceMock.Verify(x => x.GenerateJwtToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithLegacyPasswordHash_ShouldUpgradeHashAfterSuccessfulLogin()
    {
        var command = new LoginUserCommand("user@example.com", "Password123!");
        const string legacyHash = "legacy-sha256-hash";
        const string upgradedHash = "pbkdf2-sha256$600000$salt$hash";
        var user = CreateUser(legacyHash);

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.VerifyPassword(command.Password, legacyHash)).Returns(true);
        _authServiceMock.Setup(x => x.NeedsRehash(legacyHash)).Returns(true);
        _authServiceMock.Setup(x => x.HashPassword(command.Password)).Returns(upgradedHash);
        _authServiceMock.Setup(x => x.GenerateJwtToken(user.Id, user.Email.Value, user.Role)).Returns("token");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(upgradedHash, user.PasswordHash);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldRejectCredentials()
    {
        var command = new LoginUserCommand("missing@example.com", "Password123!");
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(command.Email)).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _handler.Handle(command, CancellationToken.None));

        _authServiceMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    private static User CreateUser(string passwordHash)
        => User.Create(
            "Test User",
            "user@example.com",
            passwordHash,
            "client",
            new DateTime(1990, 1, 1)).Value;
}
