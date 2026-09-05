using Moq;
using SimplePersonalFinance.Application.Commands.AccountCommands.RemoveTransaction;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Command.AccountCommands;

public class DeleteAccountTransactionCommandHandlerTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly DeleteAccountTransactionCommandHandler _handler;

    public DeleteAccountTransactionCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _currentUserMock.SetupGet(user => user.UserId).Returns(_currentUserId);
        _handler = new DeleteAccountTransactionCommandHandler(
            _accountRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ShouldThrowEntityNotFound()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new DeleteAccountTransactionCommand(transactionId, accountId);

        _accountRepositoryMock
            .Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId, CancellationToken.None))
            .ReturnsAsync((Account?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidDelete_ShouldDeleteTransactionAndUpdateBalance()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var account = new Account(_currentUserId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction("Test Transaction", 300m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);

        Assert.Equal(700m, account.CurrentBalance);
        typeof(Entity).GetProperty("Id")!.SetValue(transaction, transactionId);

        var command = new DeleteAccountTransactionCommand(transactionId, accountId);
        _accountRepositoryMock
            .Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId, CancellationToken.None))
            .ReturnsAsync(account);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(transactionId, result.Data);
        Assert.Equal(1000m, account.CurrentBalance);
        Assert.DoesNotContain(account.Transactions, item => item.IsActive);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteIncome_ShouldUpdateBalanceCorrectly()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var account = new Account(_currentUserId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction("Salary", 500m, CategoryEnum.SALARY, TransactionTypeEnum.INCOME, DateTime.Now);

        Assert.Equal(1500m, account.CurrentBalance);
        typeof(Entity).GetProperty("Id")!.SetValue(transaction, transactionId);

        var command = new DeleteAccountTransactionCommand(transactionId, accountId);
        _accountRepositoryMock
            .Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId, CancellationToken.None))
            .ReturnsAsync(account);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1000m, account.CurrentBalance);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccountBelongsToAnotherUser_ShouldThrowEntityNotFoundAndNotPersist()
    {
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Other User Account", 1000m);
        var transaction = account.AddTransaction("Test Transaction", 300m, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, DateTime.Now);
        typeof(Entity).GetProperty("Id")!.SetValue(transaction, transactionId);

        var command = new DeleteAccountTransactionCommand(transactionId, accountId);
        _accountRepositoryMock
            .Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId, CancellationToken.None))
            .ReturnsAsync(account);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
