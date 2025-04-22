using Moq;
using SimplePersonalFinance.Application.Commands.EditTransaction;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.ValueObjects;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Test.Application.Command.AccountCommands;

public class EditAccountTransactionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly EditAccountTransactionCommandHandler _handler;

    public EditAccountTransactionCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Accounts).Returns(_accountRepositoryMock.Object);
        _handler = new EditAccountTransactionCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_AccountNotFound_ShouldReturnError()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var command = new EditAccountTransactionCommand(
            transactionId,
            accountId,
            500m,
            "Updated Description",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

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
    public async Task Handle_ValidUpdate_ShouldUpdateTransactionAndReturnSuccess()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        // Create account with initial transaction
        var account = new Account(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Initial Description",
            300m,
            CategoryEnum.ENTERTAINMENT,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Set the transaction ID to match our test ID
        // This reflection is needed because the Transaction ID is set internally when AddTransaction is called
        typeof(Entity).GetProperty("Id").SetValue(transaction, transactionId);

        var command = new EditAccountTransactionCommand(
            transactionId,
            accountId,
            500m,
            "Updated Description",
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE);

        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
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
        // Arrange
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();

        // Create account with initial transaction (expense of 300)
        var account = new Account(userId, AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var transaction = account.AddTransaction(
            "Initial Description",
            300m,
            CategoryEnum.ENTERTAINMENT,
            TransactionTypeEnum.EXPENSE,
            DateTime.Now);

        // Balance should be 700 after expense
        Assert.Equal(700m, account.CurrentBalance.Amount);

        // Set the transaction ID to match our test ID
        typeof(Entity).GetProperty("Id").SetValue(transaction, transactionId);

        // Change to income with same amount
        var command = new EditAccountTransactionCommand(
            transactionId,
            accountId,
            300m,
            "Updated Description",
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME);

        _accountRepositoryMock.Setup(r => r.GetAccountWithSpecificTransactionAsync(accountId, transactionId))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        // Balance should now be 1300 (original 1000 + 300 instead of - 300)
        Assert.Equal(1300m, account.CurrentBalance.Amount);
        Assert.Equal((int)TransactionTypeEnum.INCOME, transaction.TransactionTypeId);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }
}
