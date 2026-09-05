using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class Budget : AggregateRoot
{
    public Guid UserId { get; private set; }
    public int CategoryId { get; private set; }
    public decimal LimitAmount { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    public Category Category { get; set; } = null!;
    public User User { get; set; } = null!;

    public Budget(Guid userId, CategoryEnum category, decimal limitAmount, int month, int year)
    {
        ValidateUserId(userId);
        ValidateCategory(category);
        ValidateBudgetDetails(limitAmount, month, year);

        UserId = userId;
        CategoryId = (int)category;
        LimitAmount = limitAmount;
        Month = month;
        Year = year;
    }

    public void UpdateBudget(decimal newLimitAmount, int month, int year)
    {
        ValidateBudgetDetails(newLimitAmount, month, year);

        LimitAmount = newLimitAmount;
        Month = month;
        Year = year;
    }

    private static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("Budget user id cannot be empty");
    }

    private static void ValidateCategory(CategoryEnum category)
    {
        if (!Enum.IsDefined(category))
            throw new DomainException("Budget category is invalid");
    }

    private static void ValidateBudgetDetails(decimal limitAmount, int month, int year)
    {
        if (limitAmount <= 0)
            throw new DomainException("Budget limit amount must be greater than zero");

        if (month is < 1 or > 12)
            throw new DomainException("Budget month must be between 1 and 12");

        if (year <= 0)
            throw new DomainException("Budget year must be greater than zero");
    }

    // Constructor for EF Core
    protected Budget()
    {
    }
}
