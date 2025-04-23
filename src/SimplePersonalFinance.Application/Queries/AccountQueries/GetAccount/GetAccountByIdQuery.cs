using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccount;

public class GetAccountByIdQuery:IRequest<ResultViewModel<AccountViewModel>>
{
    public Guid Id { get; set; }
    public GetAccountByIdQuery(Guid id)
    {
        Id = id;
    }
}
