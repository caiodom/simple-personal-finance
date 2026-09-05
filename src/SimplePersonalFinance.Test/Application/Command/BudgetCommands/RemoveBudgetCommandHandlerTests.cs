using Moq;
using SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Command.BudgetCommands;

public class RemoveBudgetCommandHandlerTests
{
    private readonly Guid _currentUserId = Guid.NewGuid();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly RemoveBudgetCommandHandler _handler;

    public RemoveBudgetCommandHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _currentUserMock = new Mock<ICurrentUser>();
        _unitOfWorkMock.Setup(uow => uow.Budgets).Returns(_budgetRepositoryMock.Object);
        _currentUserMock.SetupGet(user => user.UserId).Returns(_currentUserId);
        _handler = new RemoveBudgetCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBudgetExists_ShouldRemoveBudgetAndReturnSuccess()
    {
        var budgetId = Guid.NewGuid();
        var budget = new Budget(_currentUserId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new RemoveBudgetCommand(budgetId);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync(budget);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBudgetDoesNotExist_ShouldThrowEntityNotFound()
    {
        var budgetId = Guid.NewGuid();
        var command = new RemoveBudgetCommand(budgetId);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync((Budget?)null);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenBudgetBelongsToAnotherUser_ShouldThrowEntityNotFoundAndNotPersist()
    {
        var budgetId = Guid.NewGuid();
        var budget = new Budget(Guid.NewGuid(), CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var command = new RemoveBudgetCommand(budgetId);

        _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId)).ReturnsAsync(budget);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
    }
}
