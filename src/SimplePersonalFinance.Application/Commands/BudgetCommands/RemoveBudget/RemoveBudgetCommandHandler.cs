using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.BudgetCommands.RemoveBudget;

public class RemoveBudgetCommandHandler(
    IBudgetRepository budgets,
    IUnitOfWork uow,
    ICurrentUser currentUser) : IRequestHandler<RemoveBudgetCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RemoveBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await budgets.GetByIdAsync(request.Id, cancellationToken);

        if (budget is null || budget.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Budget", request.Id, "Budget not found");

        budget.SetAsDeleted();
        await uow.SaveChangesAsync(cancellationToken);

        return ResultViewModel<Guid>.Success(budget.Id, "Budget removed successfully");
    }
}
