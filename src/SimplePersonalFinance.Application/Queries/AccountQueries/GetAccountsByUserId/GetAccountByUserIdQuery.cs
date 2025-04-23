using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountsByUserId;

public class GetAccountByUserIdQuery:IRequest<ResultViewModel<PaginatedResult<AccountViewModel>>>
{
    public Guid UserId { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public GetAccountByUserIdQuery(Guid userId,int pageNumber, int pageSize)
    {
        UserId = userId;
        PageNumber=pageNumber;
        PageSize = pageSize;
    }
}
