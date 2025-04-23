using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.RemoveTransaction;

public class DeleteAccountTransactionCommandHandler(IUnitOfWork uow):IRequestHandler<DeleteAccountTransactionCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(DeleteAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetAccountWithSpecificTransactionAsync(request.AccountId, request.Id);
        if (account == null)
            return ResultViewModel<Guid>.Error("Account not found");

        account.DeleteTransaction(request.Id);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(request.Id);
    }
}

