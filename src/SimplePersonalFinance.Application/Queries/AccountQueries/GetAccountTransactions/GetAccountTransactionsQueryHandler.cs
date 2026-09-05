using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountTransactions;

public class GetAccountTransactionsQueryHandler(IUnitOfWork uow, ICurrentUser currentUser):IRequestHandler<GetAccountTransactionsQuery, ResultViewModel<AccountTransactionsViewModel>>
{
    public async Task<ResultViewModel<AccountTransactionsViewModel>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetAccountWithTransactionsAsync(request.AccountId);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        return ResultViewModel<AccountTransactionsViewModel>.Success(AccountTransactionsViewModel.MapToViewModel(account));
    }
}
