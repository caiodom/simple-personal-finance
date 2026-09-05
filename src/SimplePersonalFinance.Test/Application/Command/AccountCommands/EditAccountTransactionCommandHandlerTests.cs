using Moq;
using SimplePersonalFinance.Application.Commands.AccountCommands.EditTransaction;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Command.AccountCommands;

public class EditAccountTransactionCommandHandlerTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly EditAccountTransactionCommandHandler _handler;

    public EditAccountTransactionCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _currentUserMock.SetupGet(user => user.UserId).Returns(_currentUserId);
        _handler = new EditAccountTransactionCommandHandler(
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ShouldThrowEntityNotFound()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new EditAccountTransactionCommand(transactionId, accountId, 500m, "Updated Description", CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE);

        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId)).ReturnsAsync((Account?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidUpdate_ShouldUpdateTransactionAndReturnSuccess()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var account = new Account(_currentUserId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction("Initial Description", 300m, CategoryEnum.ENTERTAINMENT, TransactionTypeEnum.EXPENSE, DateTime.Now);
        typeof(Entity).GetProperty("Id")!.SetValue(transaction, transactionId);

        var command = new EditAccountTransactionCommand(transactionId, accountId, 500m, "Updated Description", CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE);
        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId)).ReturnsAsync(account);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(transactionId, result.Data);
        Assert.Equal(500m, transaction.Amount);
        Assert.Equal("Updated Description", transaction.Description);
        Assert.Equal((int)CategoryEnum.FOOD, transaction.CategoryId);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ChangeTransactionType_ShouldUpdateBalanceCorrectly()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var account = new Account(_currentUserId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction("Initial Description", 300m, CategoryEnum.ENTERTAINMENT, TransactionTypeEnum.EXPENSE, DateTime.Now);

        Assert.Equal(700m, account.CurrentBalance);
        typeof(Entity).GetProperty("Id")!.SetValue(transaction, transactionId);

        var command = new EditAccountTransactionCommand(transactionId, accountId, 300m, "Updated Description", CategoryEnum.SALARY, TransactionTypeEnum.INCOME);
        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId)).ReturnsAsync(account);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1300m, account.CurrentBalance);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccountBelongsToAnotherUser_ShouldThrowEntityNotFoundAndNotPersist()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Other User Account", 1000m);
        var transaction = account.AddTransaction("Initial Description", 300m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        typeof(Entity).GetProperty("Id")!.SetValue(transaction, transactionId);

        var command = new EditAccountTransactionCommand(transactionId, accountId, 500m, "Updated Description", CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE);
        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId)).ReturnsAsync(account);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
