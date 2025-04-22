using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.GetAccountTransactions;

//REMOVE
public class GetAccountTransactionsQueryHandler(IUnitOfWork uow):IRequestHandler<GetAccountTransactionsQuery, ResultViewModel<AccountTransactionsViewModel>>
{
    public async Task<ResultViewModel<AccountTransactionsViewModel>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
    {
        var accountTransactions = await uow.Accounts.GetAccountWithTransactionsAsync(request.AccountId);

        if (accountTransactions == null)
            return ResultViewModel<AccountTransactionsViewModel>.Error("Account not found");

        return ResultViewModel<AccountTransactionsViewModel>.Success(AccountTransactionsViewModel.MapToViewModel(accountTransactions));
    }
}
