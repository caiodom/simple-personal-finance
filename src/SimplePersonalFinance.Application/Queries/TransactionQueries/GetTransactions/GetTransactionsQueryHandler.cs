using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Shared.Contracts;
using SimplePersonalFinance.Shared.Extensions;

namespace SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactions;

public class GetTransactionsQueryHandler(IUnitOfWork uow) : IRequestHandler<GetTransactionsQuery, ResultViewModel<PaginatedResult<TransactionViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<TransactionViewModel>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = uow.Transactions.GetAllByAccountId(request.AccountId);

        if(transactions == null)
            throw new InvalidOperationException("No transactions found for your account");

        var results =await  transactions
                        .Select(x => TransactionViewModel.ToViewModel(x))
                        .ToPaginatedResultAsync(request.PageNumber,request.PageSize,
                            cancellationToken);

        return ResultViewModel<PaginatedResult<TransactionViewModel>>.Success(results);

    }
}
