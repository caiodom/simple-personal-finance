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
        _unitOfWorkMock.Setup(uow => uow.Users).Returns(_userRepositoryMock.Object);
        _handler = new CreateUserCommandHandler(_authServiceMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_ShouldCreateUserAndReturnSuccess()
    {
        var command = new CreateUserCommand(
            "Test User",
            "Password123!",
            "teste@example.com",
            new DateTime(1993, 3, 1));

        _userRepositoryMock.Setup(r => r.CheckEmailAsync(command.Email))
            .ReturnsAsync(false);

        _authServiceMock.Setup(x => x.HashPassword(command.Password))
            .Returns("pbkdf2-hashed-password");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _authServiceMock.Verify(x => x.HashPassword(command.Password), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.PasswordHash == "pbkdf2-hashed-password")), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnError()
    {
        var command = new CreateUserCommand(
            "Test User",
            "Password123!",
            "existing@example.com",
            new DateTime(1993, 3, 1));

        _userRepositoryMock.Setup(r => r.CheckEmailAsync(command.Email))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(
            () => _handler.Handle(command, CancellationToken.None));

        _authServiceMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
    }
}
