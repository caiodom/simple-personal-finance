using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;

public class EditBudgetCommand:IRequest<ResultViewModel<Guid>>
{
    public Guid Id { get; private set; }
    public decimal LimitAmount { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }

    public EditBudgetCommand(Guid id, decimal limitAmount, int month, int year)
    {
        Id = id;
        LimitAmount = limitAmount;
        Month = month;
        Year = year;
    }
}
