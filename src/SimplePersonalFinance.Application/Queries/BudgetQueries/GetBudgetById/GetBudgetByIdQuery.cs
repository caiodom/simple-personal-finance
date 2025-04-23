using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Budgets;

namespace SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;

public class GetBudgetByIdQuery: IRequest<ResultViewModel<BudgetViewModel>>
{
    public Guid Id { get; set; }
    public GetBudgetByIdQuery(Guid id)
    {
        Id = id;
    }
}
