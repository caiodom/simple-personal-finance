using MediatR;
using SimplePersonalFinance.Application.ViewModels;

namespace SimplePersonalFinance.Application.Queries.GetBudgetById;

public class GetBudgetByIdQuery: IRequest<ResultViewModel<BudgetViewModel>>
{
    public Guid Id { get; set; }
    public GetBudgetByIdQuery(Guid id)
    {
        Id = id;
    }
}
