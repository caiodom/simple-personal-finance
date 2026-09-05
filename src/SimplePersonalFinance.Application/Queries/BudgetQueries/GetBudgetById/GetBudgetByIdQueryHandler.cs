using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Budgets;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;

public class GetBudgetByIdQueryHandler(
    IBudgetRepository budgets,
    ICurrentUser currentUser) : IRequestHandler<GetBudgetByIdQuery, ResultViewModel<BudgetViewModel>>
{
    public async Task<ResultViewModel<BudgetViewModel>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await budgets.GetByIdAsync(request.Id, cancellationToken);

        if (budget is null || budget.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Budget", request.Id);

        return ResultViewModel<BudgetViewModel>.Success(BudgetViewModel.FromEntity(budget));
    }
}
