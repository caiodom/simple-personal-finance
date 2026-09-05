using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.RemoveTransaction;

public sealed class DeleteAccountTransactionCommandHandler(
    IAccountRepository accounts,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser) : IRequestHandler<DeleteAccountTransactionCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(
        DeleteAccountTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var account = await accounts.GetAccountWithSpecificTransactionAsync(
            request.AccountId,
            request.Id,
            cancellationToken);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        account.DeleteTransaction(request.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ResultViewModel<Guid>.Success(request.Id, "Transaction deleted successfully");
    }
}
