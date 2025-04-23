using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountTransactions;

public class GetAccountTransactionsQuery:IRequest<ResultViewModel<AccountTransactionsViewModel>>
{
    public Guid AccountId { get; private set; }
    public GetAccountTransactionsQuery(Guid accountId)
    {
        AccountId = accountId;
    }
}
