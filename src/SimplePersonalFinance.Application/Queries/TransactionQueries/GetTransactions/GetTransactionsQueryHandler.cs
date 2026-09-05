using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactions;

public class GetTransactionsQueryHandler(
    IAccountRepository accounts,
    ITransactionRepository transactions,
    ICurrentUser currentUser) : IRequestHandler<GetTransactionsQuery, ResultViewModel<PaginatedResult<TransactionViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<TransactionViewModel>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var account = await accounts.GetByIdAsync(request.AccountId, cancellationToken);
        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        var (itemsFromRepository, totalItems) = await transactions.GetAllByAccountIdAsync(
            request.AccountId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var items = itemsFromRepository
            .Select(TransactionViewModel.ToViewModel)
            .ToList();

        var result = new PaginatedResult<TransactionViewModel>(items, totalItems, request.PageNumber, request.PageSize);
        return ResultViewModel<PaginatedResult<TransactionViewModel>>.Success(result);
    }
}
