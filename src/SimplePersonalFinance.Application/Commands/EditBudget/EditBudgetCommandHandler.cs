using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.EditBudget;

public class EditBudgetCommandHandler(IUnitOfWork uow) : IRequestHandler<EditBudgetCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(EditBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await uow.Budgets.GetByIdAsync(request.Id);

        if (budget == null)
            return ResultViewModel<Guid>.Error("Budget not found");

        budget.UpdateBudget(request.LimitAmount,request.Year,request.Month);

        await uow.SaveChangesAsync();
        return ResultViewModel<Guid>.Success(request.Id);
    }
}
