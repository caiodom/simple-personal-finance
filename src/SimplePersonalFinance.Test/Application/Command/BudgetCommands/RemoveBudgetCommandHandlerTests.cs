using Moq;
using SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Test.Application.Command.BudgetCommands;

public class RemoveBudgetCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly RemoveBudgetCommandHandler _handler;

    public RemoveBudgetCommandHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(uow => uow.Budgets).Returns(_budgetRepositoryMock.Object);
        _handler = new RemoveBudgetCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBudgetExists_ShouldRemoveBudgetAndReturnSuccess()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new RemoveBudgetCommand(budgetId);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync(budget);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBudgetDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var budgetId = Guid.NewGuid();
        var command = new RemoveBudgetCommand(budgetId);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
            .ReturnsAsync((Budget)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        
    }
}
