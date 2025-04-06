using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class Budget : Entity
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

    //EF Rel
    public Category Category { get; set; }
    public User User { get; set; }
}
