using Moq;
using SimplePersonalFinance.Application.Commands.RemoveTransaction;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Test.Application.Command.AccountCommands;

public class DeleteAccountTransactionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly DeleteAccountTransactionCommandHandler _handler;

    public DeleteAccountTransactionCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Accounts).Returns(_accountRepositoryMock.Object);
        _handler = new DeleteAccountTransactionCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ShouldReturnError()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new DeleteAccountTransactionCommand(transactionId, accountId);

        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Account not found", result.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidDelete_ShouldDeleteTransactionAndUpdateBalance()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        // Create account with initial transaction
        var account = new Account(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Test Transaction",
            300m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance after expense should be 700
        Assert.Equal(700m, account.CurrentBalance);

        // Set the transaction ID to match our test ID
        typeof(Entity).GetProperty("Id").SetValue(transaction, transactionId);

        var command = new DeleteAccountTransactionCommand(transactionId, accountId);

        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(transactionId, result.Data);
        Assert.Equal(1000m, account.CurrentBalance); // Balance should be back to original
        Assert.Empty(account.Transactions); // Transaction should be removed
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DeleteIncome_ShouldUpdateBalanceCorrectly()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        // Create account with initial income transaction
        var account = new Account(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Salary",
            500m,
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME,
            DateTime.Now);

        // Balance after income should be 1500
        Assert.Equal(1500m, account.CurrentBalance);

        // Set the transaction ID to match our test ID
        typeof(Entity).GetProperty("Id").SetValue(transaction, transactionId);

        var command = new DeleteAccountTransactionCommand(transactionId, accountId);

        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1000m, account.CurrentBalance); // Balance should be back to original
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}
