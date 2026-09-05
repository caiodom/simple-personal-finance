using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.EditTransaction;

public class EditAccountTransactionCommandHandler(IUnitOfWork uow, ICurrentUser currentUser) : IRequestHandler<EditAccountTransactionCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(EditAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetAccountWithSpecificTransactionAsync(request.AccountId, request.Id);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        account.EditTransaction(request.Id, request.Amount, request.Description, request.CategoryId, request.TransactionTypeId);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(request.Id, "Transaction updated successfully");
    }
}
