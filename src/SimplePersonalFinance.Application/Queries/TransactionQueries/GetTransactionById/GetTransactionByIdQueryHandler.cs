﻿using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.TransactionQueries.GetTransactionById;

public class GetTransactionByIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetTransactionByIdQuery, ResultViewModel<TransactionViewModel>>
{
    public async Task<ResultViewModel<TransactionViewModel>> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
    {
       var transaction= await uow.Transactions.GetByIdAsync(request.Id);

        if (transaction == null)
           throw new EntityNotFoundException("Transaction", request.Id);

        var transactionViewModel = TransactionViewModel.ToViewModel(transaction);

        return ResultViewModel<TransactionViewModel>.Success(transactionViewModel);
    }
}
