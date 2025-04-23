using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Budgets;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Shared.Contracts;
using SimplePersonalFinance.Shared.Extensions;

namespace SimplePersonalFinance.Application.Queries.GetBudget;

public class GetBudgetsQueryHandler(IUnitOfWork uow) : IRequestHandler<GetBudgetsQuery, ResultViewModel<PaginatedResult<BudgetViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<BudgetViewModel>>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = uow.Budgets.GetAllByUserId(request.UserId);

        if (budgets == null)
            return ResultViewModel<PaginatedResult<BudgetViewModel>>.Error("No budgets found for this account.");

        var results = await budgets
                        .Select(x => BudgetViewModel.FromEntity(x))
                        .ToPaginatedResultAsync(request.PageNumber, request.PageSize,
                            cancellationToken);

        return ResultViewModel<PaginatedResult<BudgetViewModel>>.Success(results);
    }
}
