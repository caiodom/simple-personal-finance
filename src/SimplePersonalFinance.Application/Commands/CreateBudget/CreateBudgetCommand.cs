using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Application.Commands.CreateBudget;

public class CreateBudgetCommand:IRequest<ResultViewModel<Guid>>
{
    public Guid UserId { get; private set; }
    public CategoryEnum Category { get; private set; }

    public decimal LimitAmount { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public CreateBudgetCommand(Guid userId, CategoryEnum category,decimal limitAmount, int month, int year)
    {
        UserId = userId;
        Category = category;
        LimitAmount = limitAmount;
        Month = month;
        Year = year;
    }

    public Budget ToEntity()
    {
        return new Budget(UserId, Category, LimitAmount, Month, Year);
    }
}
