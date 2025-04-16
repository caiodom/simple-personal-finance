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
    public async Task Handle_WhenBudgetExists_ShouldUpdateAndReturnSuccess()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 200m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(budgetId, result.Data);
        Assert.Equal(200m, budget.LimitAmount);
        Assert.Equal(2, budget.Month);
        Assert.Equal(2023, budget.Year);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBudgetDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new EditBudgetCommand(budgetId, 200m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync((Budget)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Budget not found", result.Message);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
