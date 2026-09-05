using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Test.Core.Domain.Entities;

public class AggregateRootDomainEventsTests
{
    [Fact]
    public void DomainEvents_ShouldAlwaysExposeANonNullCollection()
    {
        var account = new Account(
            Guid.NewGuid(),
            AccountTypeEnum.CHECKING,
            "Test Account",
            100m);

        Assert.NotNull(account.DomainEvents);
        Assert.Empty(account.DomainEvents);
    }

    [Fact]
    public void ClearDomainEvents_ShouldLeaveTheCollectionEmptyAndReusable()
    {
        var account = new Account(
            Guid.NewGuid(),
            AccountTypeEnum.CHECKING,
            "Test Account",
            100m);

        account.AddTransaction(
            "Groceries",
            10m,
            CategoryEnum.FOOD,
            TransactionTypeEnum.EXPENSE,
            DateTime.UtcNow);

        Assert.Single(account.DomainEvents);

        account.ClearDomainEvents();

        Assert.NotNull(account.DomainEvents);
        Assert.Empty(account.DomainEvents);

        account.AddTransaction(
            "Salary",
            20m,
            CategoryEnum.SALARY,
            TransactionTypeEnum.INCOME,
            DateTime.UtcNow);

        Assert.Single(account.DomainEvents);
    }
}
