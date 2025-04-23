using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.RemoveAccount;

public class RemoveAccountCommandHandler(IUnitOfWork uow) : IRequestHandler<RemoveAccountCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(RemoveAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetFullAccountWithTransactionsAsync(request.Id);

        if (account == null)
            throw new EntityNotFoundException("Account",request.Id);

        account.DeleteAccount();

        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(account.Id);
    }
}

