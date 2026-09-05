using Moq;
using SimplePersonalFinance.Application.Commands.AccountCommands.CreateTransaction;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Command.TransactionCommands;

public class CreateTransactionCommandHandlerTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly CreateAccountTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _unitOfWorkMock.Setup(uow => uow.Accounts).Returns(_accountRepositoryMock.Object);
        _currentUserMock.SetupGet(user => user.UserId).Returns(_currentUserId);
        _handler = new CreateAccountTransactionCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidIncomeTransaction_ShouldCreateAndUpdateBalance()
    {
        var accountId = Guid.NewGuid();
        var account = new Account(_currentUserId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var command = new CreateAccountTransactionCommand(accountId, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, "Salary", 500m, DateTime.Now);

        _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        Assert.Equal(1500m, account.CurrentBalance);
        _accountRepositoryMock.Verify(r => r.AddAccountTransaction(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidExpenseTransaction_ShouldCreateAndUpdateBalance()
    {
        var accountId = Guid.NewGuid();
        var account = new Account(_currentUserId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var command = new CreateAccountTransactionCommand(accountId, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, "Groceries", 200m, DateTime.Now);

        _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        Assert.Equal(800m, account.CurrentBalance);
        _accountRepositoryMock.Verify(r => r.AddAccountTransaction(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccountNotFound_ShouldThrowEntityNotFound()
    {
        var accountId = Guid.NewGuid();
        var command = new CreateAccountTransactionCommand(accountId, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, "Groceries", 200m, DateTime.Now);

        _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync((Account?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenAccountBelongsToAnotherUser_ShouldThrowEntityNotFoundAndNotPersist()
    {
        var accountId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Other User Account", 1000m);
        var command = new CreateAccountTransactionCommand(accountId, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, "Groceries", 200m, DateTime.Now);

        _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId)).ReturnsAsync(account);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _accountRepositoryMock.Verify(r => r.AddAccountTransaction(It.IsAny<Transaction>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
