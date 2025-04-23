using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;

public class RemoveBudgetCommandHandler(IUnitOfWork uow):IRequestHandler<RemoveBudgetCommand, ResultViewModel>
{
    public async Task<ResultViewModel> Handle(RemoveBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget= await uow.Budgets.GetByIdAsync(request.Id);

        if (budget == null)
            return ResultViewModel.Error("Budget not found");

        budget.SetAsDeleted();

        await uow.SaveChangesAsync();

        return ResultViewModel.Success("Budget removed successfully");
    }
}   

