﻿using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountTransactions;

//REMOVE
public class GetAccountTransactionsQueryHandler(IUnitOfWork uow):IRequestHandler<GetAccountTransactionsQuery, ResultViewModel<AccountTransactionsViewModel>>
{
    public async Task<ResultViewModel<AccountTransactionsViewModel>> Handle(GetAccountTransactionsQuery request, CancellationToken cancellationToken)
    {
        var accountTransactions = await uow.Accounts.GetAccountWithTransactionsAsync(request.AccountId);

        if (accountTransactions == null)
            throw new EntityNotFoundException("Account", request.AccountId);

        return ResultViewModel<AccountTransactionsViewModel>.Success(AccountTransactionsViewModel.MapToViewModel(accountTransactions));
    }
}
