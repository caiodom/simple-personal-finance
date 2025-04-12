using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.GetBudgetById;

public class GetBudgetByIdQueryHandler(IUnitOfWork uow):IRequestHandler<GetBudgetByIdQuery, ResultViewModel<BudgetViewModel>>
{
    public async Task<ResultViewModel<BudgetViewModel>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await uow.Budgets.GetById(request.Id);

        if (budget == null)
            return ResultViewModel<BudgetViewModel>.Error("Budget not found");

        return ResultViewModel<BudgetViewModel>.Success(BudgetViewModel.FromEntity(budget));
    }
}
