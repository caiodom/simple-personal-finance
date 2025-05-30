﻿using Moq;
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
        _unitOfWorkMock.Setup(uow => uow.Budgets).Returns(_budgetRepositoryMock.Object);
        _handler = new CreateBudgetCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBudgetDoesNotExist_ShouldCreateAndReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBudgetCommand(userId,CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        

        _budgetRepositoryMock.Setup(r => r.GetByUserAndCategoryAsync(userId, (int)CategoryEnum.ENTERTAINMENT))
            .ReturnsAsync((Budget)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Data);
        _budgetRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Budget>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBudgetAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateBudgetCommand(userId,CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
        var existingBudget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);

        _budgetRepositoryMock.Setup(r => r.GetByUserAndCategoryAsync(It.IsAny<Guid>(),It.IsAny<int>()))
            .ReturnsAsync(existingBudget);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleViolationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
