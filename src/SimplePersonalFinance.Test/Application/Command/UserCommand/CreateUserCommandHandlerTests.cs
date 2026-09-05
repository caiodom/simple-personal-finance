using Moq;
using SimplePersonalFinance.Application.Commands.UserCommands.CreateUser;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Command.UserCommands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _authServiceMock = new Mock<IAuthService>();
        _handler = new CreateUserCommandHandler(
            _authServiceMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_ShouldHashPasswordAndCreateUser()
    {
        var command = new CreateUserCommand(
            "Test User",
            "Password123!",
            "teste@example.com",
            new DateTime(1993, 3, 1));
        const string passwordHash = "PBKDF2-SHA256$600000$salt$hash";
        var cancellationToken = new CancellationTokenSource().Token;

        _userRepositoryMock.Setup(r => r.CheckEmailAsync(command.Email, cancellationToken)).ReturnsAsync(false);
        _authServiceMock.Setup(service => service.HashPassword(command.Password)).Returns(passwordHash);

        User? savedUser = null;
        _userRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<User>(), cancellationToken))
            .Callback<User, CancellationToken>((user, _) => savedUser = user)
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        Assert.NotNull(savedUser);
        Assert.Equal(passwordHash, savedUser.PasswordHash);
        _authServiceMock.Verify(service => service.HashPassword(command.Password), Times.Once);
        _userRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>(), cancellationToken), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldThrowAndNotHashPassword()
    {
        var command = new CreateUserCommand(
            "Test User",
            "Password123!",
            "existing@example.com",
            new DateTime(1993, 3, 1));

        _userRepositoryMock.Setup(r => r.CheckEmailAsync(command.Email, CancellationToken.None)).ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(command, CancellationToken.None));

        _authServiceMock.Verify(service => service.HashPassword(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
