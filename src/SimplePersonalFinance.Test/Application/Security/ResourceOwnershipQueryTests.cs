using Moq;
using SimplePersonalFinance.Application.Queries.AccountQueries.GetAccount;
using SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;
using SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactionById;
using SimplePersonalFinance.Application.Queries.UserQueries.GetUser;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Test.Application.Security;

public class ResourceOwnershipQueryTests
{
    [Fact]
    public async Task GetAccount_WhenOwnedByAnotherUser_ShouldReturnNotFoundSemantics()
    {
        var accountId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Other Account", 100m);
        var accountRepository = new Mock<IAccountRepository>();
        var currentUser = new Mock<ICurrentUser>();

        accountRepository.Setup(repository => repository.GetByIdAsync(accountId, CancellationToken.None)).ReturnsAsync(account);
        currentUser.SetupGet(user => user.UserId).Returns(currentUserId);

        var handler = new GetAccountByIdQueryHandler(accountRepository.Object, currentUser.Object);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => handler.Handle(new GetAccountByIdQuery(accountId), CancellationToken.None));
    }

    [Fact]
    public async Task GetBudget_WhenOwnedByAnotherUser_ShouldReturnNotFoundSemantics()
    {
        var budgetId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var budget = new Budget(Guid.NewGuid(), CategoryEnum.FOOD, 500m, 9, 2026);
        var budgetRepository = new Mock<IBudgetRepository>();
        var currentUser = new Mock<ICurrentUser>();

        budgetRepository.Setup(repository => repository.GetByIdAsync(budgetId, CancellationToken.None)).ReturnsAsync(budget);
        currentUser.SetupGet(user => user.UserId).Returns(currentUserId);

        var handler = new GetBudgetByIdQueryHandler(budgetRepository.Object, currentUser.Object);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => handler.Handle(new GetBudgetByIdQuery(budgetId), CancellationToken.None));
    }

    [Fact]
    public async Task GetTransaction_WhenAccountOwnedByAnotherUser_ShouldReturnNotFoundSemantics()
    {
        var transactionId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();
        var transaction = new Transaction(accountId, CategoryEnum.FOOD, TransactionTypeEnum.EXPENSE, "Lunch", 50m, DateTime.UtcNow);
        var account = new Account(Guid.NewGuid(), AccountTypeEnum.CHECKING, "Other Account", 100m);
        var transactionRepository = new Mock<ITransactionRepository>();
        var accountRepository = new Mock<IAccountRepository>();
        var currentUser = new Mock<ICurrentUser>();

        transactionRepository.Setup(repository => repository.GetByIdAsync(transactionId, CancellationToken.None)).ReturnsAsync(transaction);
        accountRepository.Setup(repository => repository.GetByIdAsync(accountId, CancellationToken.None)).ReturnsAsync(account);
        currentUser.SetupGet(user => user.UserId).Returns(currentUserId);

        var handler = new GetTransactionByIdQueryHandler(
            transactionRepository.Object,
            accountRepository.Object,
            currentUser.Object);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => handler.Handle(new GetTransactionByIdQuery(transactionId), CancellationToken.None));
    }

    [Fact]
    public async Task GetUser_WhenRequestingAnotherUser_ShouldReturnNotFoundSemanticsWithoutRepositoryAccess()
    {
        var currentUserId = Guid.NewGuid();
        var requestedUserId = Guid.NewGuid();
        var userRepository = new Mock<IUserRepository>();
        var currentUser = new Mock<ICurrentUser>();

        currentUser.SetupGet(user => user.UserId).Returns(currentUserId);
        var handler = new GetUserQueryHandler(userRepository.Object, currentUser.Object);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => handler.Handle(new GetUserQuery(requestedUserId), CancellationToken.None));
        userRepository.Verify(
            repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
