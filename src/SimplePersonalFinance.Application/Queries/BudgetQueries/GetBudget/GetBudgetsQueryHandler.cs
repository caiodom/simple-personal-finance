using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Budgets;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudget;

public class GetBudgetsQueryHandler(IUnitOfWork uow) : IRequestHandler<GetBudgetsQuery, ResultViewModel<PaginatedResult<BudgetViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<BudgetViewModel>>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
    {
        var (budgets, totalItems) = await uow.Budgets.GetAllByUserIdAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize);

        var items = budgets
            .Select(BudgetViewModel.FromEntity)
            .ToList();

        var result = new PaginatedResult<BudgetViewModel>(items, totalItems, request.PageNumber, request.PageSize);
        return ResultViewModel<PaginatedResult<BudgetViewModel>>.Success(result);
    }
}
