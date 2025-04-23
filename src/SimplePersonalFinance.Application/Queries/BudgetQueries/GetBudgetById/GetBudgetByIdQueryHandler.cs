using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Budgets;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.BudgetQueries.GetBudgetById;

public class GetBudgetByIdQueryHandler(IUnitOfWork uow):IRequestHandler<GetBudgetByIdQuery, ResultViewModel<BudgetViewModel>>
{
    public async Task<ResultViewModel<BudgetViewModel>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await uow.Budgets.GetByIdAsync(request.Id);
        if (budget == null)
            return ResultViewModel<BudgetViewModel>.NotFound("Budget not found");

        return ResultViewModel<BudgetViewModel>.Success(BudgetViewModel.FromEntity(budget));
    }
}
