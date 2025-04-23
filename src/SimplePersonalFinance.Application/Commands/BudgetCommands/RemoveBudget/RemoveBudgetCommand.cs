using MediatR;
using SimplePersonalFinance.Application.ViewModels;

namespace SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;

public class RemoveBudgetCommand:IRequest<ResultViewModel>
{
    public Guid Id { get; private set; }
    public RemoveBudgetCommand(Guid id)
    {
        Id = id;
    }
}
