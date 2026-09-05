using Moq;
using SimplePersonalFinance.Application.Commands.BudgetCommands.CreateBudget;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Test.Application.Command.BudgetCommands;

public class CreateBudgetCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
    private readonly CreateBudgetCommandHandler _handler;

    public CreateBudgetCommandHandlerTests()
    {
        _budgetRepositoryMock = new Mock<IBudgetRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateBudgetCommandHandler(_budgetRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBudgetDoesNotExist_ShouldCreateAndReturnSuccess()
    {
        var userId = Guid.NewGuid();
        var command = new CreateBudgetCommand(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);

        _budgetRepositoryMock
            .Setup(r => r.GetByUserAndCategoryAsync(userId, (int)CategoryEnum.ENTERTAINMENT, CancellationToken.None))
            .ReturnsAsync((Budget?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _budgetRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Budget>(), CancellationToken.None), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBudgetAlreadyExists_ShouldReturnError()
    {
        var userId = Guid.NewGuid();
        var command = new CreateBudgetCommand(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var existingBudget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);

        _budgetRepositoryMock
            .Setup(r => r.GetByUserAndCategoryAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBudget);

        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(command, CancellationToken.None));
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
