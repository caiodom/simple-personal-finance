using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactionById;

public class GetTransactionByIdQueryHandler(
    ITransactionRepository transactions,
    IAccountRepository accounts,
    ICurrentUser currentUser) : IRequestHandler<GetTransactionByIdQuery, ResultViewModel<TransactionViewModel>>
{
    public async Task<ResultViewModel<TransactionViewModel>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await transactions.GetByIdAsync(request.Id);

        if (transaction is null)
            throw new EntityNotFoundException("Transaction", request.Id);

        var account = await accounts.GetByIdAsync(transaction.AccountId);
        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Transaction", request.Id);

        return ResultViewModel<TransactionViewModel>.Success(TransactionViewModel.ToViewModel(transaction));
    }
}
