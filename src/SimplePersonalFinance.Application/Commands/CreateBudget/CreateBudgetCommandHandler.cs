using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.CreateBudget;

public class CreateBudgetCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateBudgetCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var existingBudget = await uow.Budgets.GetByUserAndCategoryAsync(request.UserId, (int)request.Category);
        if (existingBudget != null)
            return ResultViewModel<Guid>.Error("Budget already exists for this category and user.");

        var budget = request.ToEntity();

        await uow.Budgets.AddAsync(budget);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(budget.Id);
    }
}
