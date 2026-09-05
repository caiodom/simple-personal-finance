using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;
using SimplePersonalFinance.Shared.Contracts;
using SimplePersonalFinance.Shared.Extensions;

namespace SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactions;

public class GetTransactionsQueryHandler(IUnitOfWork uow, ICurrentUser currentUser) : IRequestHandler<GetTransactionsQuery, ResultViewModel<PaginatedResult<TransactionViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<TransactionViewModel>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetByIdAsync(request.AccountId);
        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        var transactions = uow.Transactions.GetAllByAccountId(request.AccountId);

        if (transactions == null)
            throw new InvalidOperationException("No transactions found for your account");

        var results = await transactions
            .Select(x => TransactionViewModel.ToViewModel(x))
            .ToPaginatedResultAsync(request.PageNumber, request.PageSize, cancellationToken);

        return ResultViewModel<PaginatedResult<TransactionViewModel>>.Success(results);
    }
}
