using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;

public class RemoveBudgetCommandHandler(IUnitOfWork uow):IRequestHandler<RemoveBudgetCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RemoveBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget= await uow.Budgets.GetByIdAsync(request.Id);
        if (budget == null)
            throw new EntityNotFoundException("Budget",request.Id, "Budget not found");

        budget.SetAsDeleted();
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(budget.Id,"Budget removed successfully");
    }
}   

