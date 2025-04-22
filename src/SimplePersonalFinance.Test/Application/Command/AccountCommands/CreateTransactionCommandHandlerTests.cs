using Moq;
using SimplePersonalFinance.Application.Commands.CreateTransaction;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.ValueObjects;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Test.Application.Command.TransactionCommands;

public class CreateTransactionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly CreateAccountTransactionCommandHandler _handler;

    public CreateTransactionCommandHandlerTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Accounts).Returns(_accountRepositoryMock.Object);
        _handler = new CreateAccountTransactionCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidIncomeTransaction_ShouldCreateAndUpdateBalance()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var command = new CreateAccountTransactionCommand(accountId,CategoryEnum.SALARY,TransactionTypeEnum.INCOME,"Salary", 500m, DateTime.Now);

        
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        Assert.Equal(1500m, account.CurrentBalance.Amount); // Balance increased
        _accountRepositoryMock.Verify(r => r.AddAccountTransaction(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidExpenseTransaction_ShouldCreateAndUpdateBalance()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Test Account", 1000m);
        var command = new CreateAccountTransactionCommand(accountId, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, "Groceries", 200m, DateTime.Now);


        _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync(account);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        Assert.Equal(800m, account.CurrentBalance.Amount); // Balance decreased
        _accountRepositoryMock.Verify(r => r.AddAccountTransaction(It.IsAny<Transaction>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAccountNotFound_ShouldReturnError()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        var command = new CreateAccountTransactionCommand(accountId, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, "Groceries", 200m, DateTime.Now);

        _accountRepositoryMock.Setup(r => r.GetByIdAsync(accountId))
            .ReturnsAsync((Account)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Account not found", result.Message);
        _accountRepositoryMock.Verify(r => r.AddAccountTransaction(It.IsAny<Transaction>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
