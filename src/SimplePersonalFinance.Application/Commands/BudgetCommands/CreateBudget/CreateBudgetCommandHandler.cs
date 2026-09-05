using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Application.Commands.BudgetCommands.CreateBudget;

public class CreateBudgetCommandHandler(
    IBudgetRepository budgets,
    IUnitOfWork uow) : IRequestHandler<CreateBudgetCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var existingBudget = await budgets.GetByUserAndCategoryAsync(request.UserId, (int)request.Category);
        if (existingBudget != null)
            throw new BusinessRuleViolationException(
                "Budget already exists",
                "A budget already exists for this category and user. Please remove the existing budget before creating a new one.");

        var budget = request.ToEntity();

        await budgets.AddAsync(budget);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(budget.Id, "Budget created successfully");
    }
}
