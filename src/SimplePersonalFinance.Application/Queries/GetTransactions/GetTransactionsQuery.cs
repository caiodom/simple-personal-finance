using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.GetTransactions;

public class GetTransactionsQuery:IRequest<ResultViewModel<PaginatedResult<TransactionViewModel>>>
{
    public Guid AccountId { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public GetTransactionsQuery(Guid accountId, int pageNumber, int pageSize)
    {
        AccountId = accountId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}


