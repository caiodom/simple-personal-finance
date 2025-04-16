using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.EditTransaction;

public class EditAccountTransactionCommandHandler(IUnitOfWork uow) : IRequestHandler<EditAccountTransactionCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(EditAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetAccountWithSpecificTransactionAsync(request.AccountId, request.Id);
        if (account == null)
            return ResultViewModel<Guid>.Error("Account not found");

        account.EditTransaction(request.Id, request.Amount, request.Description, request.CategoryId, request.TransactionTypeId);

        await uow.SaveChangesAsync();
        return ResultViewModel<Guid>.Success(request.Id);
    }
}

