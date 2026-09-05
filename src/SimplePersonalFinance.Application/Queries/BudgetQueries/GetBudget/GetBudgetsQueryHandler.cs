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
        var page = await uow.Budgets.GetAllByUserIdAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var items = page.Items
            .Select(BudgetViewModel.FromEntity)
            .ToList();

        var result = new PaginatedResult<BudgetViewModel>(
            items,
            page.TotalItems,
            request.PageNumber,
            request.PageSize);

        return ResultViewModel<PaginatedResult<BudgetViewModel>>.Success(result);
    }
}
