using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Budgets;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudget;

public class GetBudgetsQueryHandler(IBudgetRepository budgets) : IRequestHandler<GetBudgetsQuery, ResultViewModel<PaginatedResult<BudgetViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<BudgetViewModel>>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var (itemsFromRepository, totalItems) = await budgets.GetAllByUserIdAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize);

        var items = itemsFromRepository
            .Select(BudgetViewModel.FromEntity)
            .ToList();

        var result = new PaginatedResult<BudgetViewModel>(items, totalItems, request.PageNumber, request.PageSize);
        return ResultViewModel<PaginatedResult<BudgetViewModel>>.Success(result);
    }
}
