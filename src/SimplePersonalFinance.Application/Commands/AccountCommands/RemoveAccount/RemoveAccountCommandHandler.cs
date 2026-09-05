using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.RemoveAccount;

public class RemoveAccountCommandHandler(
    IAccountRepository accounts,
    IUnitOfWork uow,
    ICurrentUser currentUser) : IRequestHandler<RemoveAccountCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RemoveAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accounts.GetFullAccountWithTransactionsAsync(request.Id, cancellationToken);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.Id);

        account.DeleteAccount();
        await uow.SaveChangesAsync(cancellationToken);

        return ResultViewModel<Guid>.Success(account.Id, "Account removed successfully");
    }
}
