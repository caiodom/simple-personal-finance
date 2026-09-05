using Moq;
using SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Tests.Application.Commands;

public class EditBudgetCommandHandlerTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly EditBudgetCommandHandler _handler;

    public EditBudgetCommandHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _unitOfWorkMock.Setup(uow => uow.Budgets).Returns(_budgetRepositoryMock.Object);
        _currentUserMock.SetupGet(user => user.UserId).Returns(_currentUserId);
        _handler = new EditBudgetCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_WithNegativeAmount_ShouldThrowDomainException()
    {
        var budgetId = Guid.NewGuid();
        var budget = new Budget(_currentUserId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, -50m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync(budget);

        await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithSameValues_ShouldUpdateSuccessfully()
    {
        var budgetId = Guid.NewGuid();
        var budget = new Budget(_currentUserId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 100m, 1, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync(budget);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(budgetId, result.Data);
        Assert.Equal(100m, budget.LimitAmount);
        Assert.Equal(1, budget.Month);
        Assert.Equal(2023, budget.Year);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveFails_ShouldPropagateException()
    {
        var budgetId = Guid.NewGuid();
        var budget = new Budget(_currentUserId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 200m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync(budget);
        _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync()).ThrowsAsync(new Exception("Database error"));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenBudgetBelongsToAnotherUser_ShouldThrowEntityNotFoundAndNotPersist()
    {
        var budgetId = Guid.NewGuid();
        var budget = new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new EditBudgetCommand(budgetId, 200m, 2, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync(budget);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
