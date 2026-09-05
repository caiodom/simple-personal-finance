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
        _handler = new CreateAccountCommandHandler(_accountRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidAccount_ShouldCreateAndReturnSuccess()
    {
        var userId = Guid.NewGuid();
        var command = new CreateAccountCommand(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var cancellationToken = new CancellationTokenSource().Token;

        var result = await _handler.Handle(command, cancellationToken);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>(), cancellationToken), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(cancellationToken), Times.Once);
    }
}
