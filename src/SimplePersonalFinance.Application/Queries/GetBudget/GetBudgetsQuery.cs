using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.GetBudget;

public class GetBudgetsQuery: IRequest<ResultViewModel<PaginatedResult<BudgetViewModel>>> 
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public GetBudgetsQuery(Guid userId)
    {
        UserId = userId;
    }
}
