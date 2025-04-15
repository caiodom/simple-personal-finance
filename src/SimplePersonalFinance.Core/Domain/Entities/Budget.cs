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
    
    public Budget(Guid userId, CategoryEnum category, decimal limitAmount, int month, int year)
    {
        UserId = userId;
        CategoryId = (int)category;
        LimitAmount = limitAmount;
        Month = month;
        Year = year;
    }

    public void UpdateBudget(decimal newLimitAmount, int month, int year)
    {
        if (newLimitAmount <= 0)
            throw new DomainException("Budget limit amount must be greater than zero");


        LimitAmount = newLimitAmount;
        Month = month;
        Year = year;
    }



    // Constructor for EF Core
    protected Budget() { }

    //EF Rel
    public Category Category { get; set; }
    public User User { get; set; }
}
