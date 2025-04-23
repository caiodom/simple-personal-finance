using Moq;
using SimplePersonalFinance.Application.Commands.CreateUser;
using SimplePersonalFinance.Core.Domain.Entities;
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
        _authServiceMock= new Mock<IAuthService>();
        _unitOfWorkMock.Setup(uow => uow.Users).Returns(_userRepositoryMock.Object);
        _handler = new CreateUserCommandHandler(_authServiceMock.Object,_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithUniqueEmail_ShouldCreateUserAndReturnSuccess()
    {
        // Arrange
        // Arrange
        var command = new CreateUserCommand("Test User", "Password123!", "teste@example.com",  new DateTime(1993, 3, 1));

        _userRepositoryMock.Setup(r => r.CheckEmailAsync(command.Email))
            .ReturnsAsync(false);

        _authServiceMock.Setup(x=>x.ComputeSha256Hash(It.IsAny<string>()))
                        .Returns("hashedPassword");


        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnError()
    {
        // Arrange
        var command = new CreateUserCommand("Test User", "existing@example.com", "Password123!", new DateTime(1993, 3, 1));

        _userRepositoryMock.Setup(r => r.CheckEmailAsync(command.Email))
            .ReturnsAsync(true); // Email already exists

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Email already exists", result.Message);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
