﻿using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccount;

public class GetAccountByIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAccountByIdQuery, ResultViewModel<AccountViewModel>>
{
    public async Task<ResultViewModel<AccountViewModel>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account =await uow.Accounts.GetByIdAsync(request.Id);
        if(account is null)
            throw new EntityNotFoundException("Account", request.Id);

        return ResultViewModel<AccountViewModel>.Success(AccountViewModel.MapToViewModel(account));
    }
}
