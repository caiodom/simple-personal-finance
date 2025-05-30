﻿using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.BudgetCommands.EditBudget;

public class EditBudgetCommandHandler(IUnitOfWork uow) : IRequestHandler<EditBudgetCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(EditBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await uow.Budgets.GetByIdAsync(request.Id);

        if (budget == null)
            throw new EntityNotFoundException("Budget", request.Id, "Budget not found");

        budget.UpdateBudget(request.LimitAmount,request.Month, request.Year);

        await uow.SaveChangesAsync();
        return ResultViewModel<Guid>.Success(request.Id, "Budget updated successfully");
    }
}
