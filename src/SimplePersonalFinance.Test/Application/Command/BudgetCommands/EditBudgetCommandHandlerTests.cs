using Moq;
using SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
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
    public async Task Handle_WithNegativeAmount_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, -50m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, CancellationToken.None));
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

        

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
        
    }
}
