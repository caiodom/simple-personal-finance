using Moq;
using SimplePersonalFinance.Application.Commands.AccountCommands.CreateAccount;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Test.Application.Command.AccountCommands;

public class CreateAccountCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly CreateAccountCommandHandler _handler;

    public CreateAccountCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Accounts).Returns(_accountRepositoryMock.Object);
        _handler = new CreateAccountCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidAccount_ShouldCreateAndReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateAccountCommand(userId,AccountTypeEnum.CHECKING, "Test Account", 1000M);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}
