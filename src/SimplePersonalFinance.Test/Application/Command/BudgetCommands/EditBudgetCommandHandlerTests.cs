using Moq;
using SimplePersonalFinance.Application.Commands.EditBudget;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Tests.Application.Commands;

public class EditBudgetCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly EditBudgetCommandHandler _handler;

    public EditBudgetCommandHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Budgets).Returns(_budgetRepositoryMock.Object);
        _handler = new EditBudgetCommandHandler(_unitOfWorkMock.Object);
    }


    [Fact]
    public async Task Handle_WithZeroAmount_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 0m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Amount must be greater than zero", result.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNegativeAmount_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, -50m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Amount must be greater than zero", result.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithInvalidMonth_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 200m, 13, 2023); // Invalid month

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Month must be between 1 and 12", result.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithPastYear_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var pastYear = DateTime.Now.Year - 2;
        var command = new EditBudgetCommand(budgetId, 200m, 2, pastYear);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Cannot set budget for past years", result.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_WithSameValues_ShouldUpdateSuccessfully()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 100m, 1, 2023); // Same values

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(budgetId, result.Data);
        Assert.Equal(100m, budget.LimitAmount);
        Assert.Equal(1, budget.Month);
        Assert.Equal(2023, budget.Year);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionOccurs_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 200m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("Error updating budget", result.Message);
    }
}
