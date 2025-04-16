using Moq;
using SimplePersonalFinance.Application.Queries.GetBudgetById;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Tests.Application.Queries
{
    public class GetBudgetByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBudgetRepository> _budgetRepositoryMock;
        private readonly GetBudgetByIdQueryHandler _handler;

        public GetBudgetByIdQueryHandlerTests()
        {
            _budgetRepositoryMock = new Mock<IBudgetRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _unitOfWorkMock.Setup(uow => uow.Budgets).Returns(_budgetRepositoryMock.Object);
            _handler = new GetBudgetByIdQueryHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenBudgetExists_ShouldReturnBudgetViewModel()
        {
            // Arrange
            var budgetId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var budget = new Budget(userId, CategoryEnum.ENTERTAINMENT, 100m, 1, 2023);
            budget.Category= new Category(1, "Entertainment");
            var query = new GetBudgetByIdQuery(budgetId);

            _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
                .ReturnsAsync(budget);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(budget.Category.Id, result.Data.CategoryId);
            Assert.Equal(budget.Category.Name, result.Data.CategoryName);
            Assert.Equal(budget.LimitAmount, result.Data.LimitAmount);
            Assert.Equal(budget.Month, result.Data.Month);
            Assert.Equal(budget.Year, result.Data.Year);
        }

        [Fact]
        public async Task Handle_WhenBudgetDoesNotExist_ShouldReturnError()
        {
            // Arrange
            var budgetId = Guid.NewGuid();
            var query = new GetBudgetByIdQuery(budgetId);

            _budgetRepositoryMock.Setup(r => r.GetByIdAsync(budgetId))
                .ReturnsAsync((Budget)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Budget not found", result.Message);
            Assert.Null(result.Data);
        }
    }
}
