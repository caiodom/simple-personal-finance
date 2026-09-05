using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.RemoveTransaction;

public class DeleteAccountTransactionCommandHandler(
    IAccountRepository accounts,
    IUnitOfWork uow,
    ICurrentUser currentUser) : IRequestHandler<DeleteAccountTransactionCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(DeleteAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await accounts.GetAccountWithSpecificTransactionAsync(request.AccountId, request.Id);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        account.DeleteTransaction(request.Id);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(request.Id, "Transaction deleted successfully");
    }
}
