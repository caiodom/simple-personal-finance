using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.RemoveAccount;

public class RemoveAccountCommandHandler(IUnitOfWork uow) : IRequestHandler<RemoveAccountCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RemoveAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetFullAccountWithTransactionsAsync(request.Id);

        if (account == null)
            return ResultViewModel<Guid>.Error("Account not found");

        account.DeleteAccount();
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(account.Id, "Account removed successfully");
    }
}

