using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;

namespace SimplePersonalFinance.Application.Queries.GetTransactionById;

public class GetTransactionByIdQuery:IRequest<ResultViewModel<TransactionViewModel>>
{
    public Guid Id { get; private set; }

    public GetTransactionByIdQuery(Guid id)
    {
        Id = id;
    }
}
