using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Queries.GetAccountsByUserId;

public class GetAccountByUserIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAccountByUserIdQuery, ResultViewModel<List<AccountViewModel>>>
{
    public async Task<ResultViewModel<List<AccountViewModel>>> Handle(GetAccountByUserIdQuery request, CancellationToken cancellationToken)
    {
        var accounts = await uow.Accounts.GetAccountsByUserIdAsync(request.UserId);

        if (accounts is null)
            return ResultViewModel<List<AccountViewModel>>.Error("Accounts not found");

        return ResultViewModel<List<AccountViewModel>>.Success(accounts.Select(a => AccountViewModel.MapToViewModel(a)).ToList());
    }
}
