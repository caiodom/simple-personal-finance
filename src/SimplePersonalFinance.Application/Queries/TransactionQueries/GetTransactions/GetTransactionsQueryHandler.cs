using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactions;

public class GetTransactionsQueryHandler(IUnitOfWork uow, ICurrentUser currentUser) : IRequestHandler<GetTransactionsQuery, ResultViewModel<PaginatedResult<TransactionViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<TransactionViewModel>>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetByIdAsync(request.AccountId);
        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        var page = await uow.Transactions.GetAllByAccountIdAsync(
            request.AccountId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var items = page.Items
            .Select(TransactionViewModel.ToViewModel)
            .ToList();

        var result = new PaginatedResult<TransactionViewModel>(
            items,
            page.TotalItems,
            request.PageNumber,
            request.PageSize);

        return ResultViewModel<PaginatedResult<TransactionViewModel>>.Success(result);
    }
}
