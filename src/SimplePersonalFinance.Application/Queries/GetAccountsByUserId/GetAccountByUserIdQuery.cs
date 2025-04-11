using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;

namespace SimplePersonalFinance.Application.Queries.GetAccountsByUserId;

public class GetAccountByUserIdQuery:IRequest<ResultViewModel<List<AccountViewModel>>>
{
    public Guid UserId { get; set; }
    public GetAccountByUserIdQuery(Guid userId)
    {
        UserId = userId;
    }
}
