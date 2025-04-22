using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.GetAccountsByUserId;

public class GetAccountByUserIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAccountByUserIdQuery, ResultViewModel<List<AccountViewModel>>>
{
    public async Task<ResultViewModel<List<AccountViewModel>>> Handle(GetAccountByUserIdQuery request, CancellationToken cancellationToken)
    {
        var accounts = await uow.Accounts.GetAccountsByUserIdAsync(request.UserId);

        if (accounts == null)
            throw new EntityNotFoundException("Account", request.UserId);

        return ResultViewModel<List<AccountViewModel>>.Success(accounts.Select(a => AccountViewModel.MapToViewModel(a)).ToList());
    }
}
