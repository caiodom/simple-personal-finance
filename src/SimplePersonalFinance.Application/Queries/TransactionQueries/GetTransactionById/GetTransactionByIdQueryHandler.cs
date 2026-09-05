using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactionById;

public class GetTransactionByIdQueryHandler(IUnitOfWork uow, ICurrentUser currentUser) : IRequestHandler<GetTransactionByIdQuery, ResultViewModel<TransactionViewModel>>
{
    public async Task<ResultViewModel<TransactionViewModel>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
        var transaction = await uow.Transactions.GetByIdAsync(request.Id);

        if (transaction is null)
            throw new EntityNotFoundException("Transaction", request.Id);

        var account = await uow.Accounts.GetByIdAsync(transaction.AccountId);
        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Transaction", request.Id);

        var transactionViewModel = TransactionViewModel.ToViewModel(transaction);

        return ResultViewModel<TransactionViewModel>.Success(transactionViewModel);
    }
}
