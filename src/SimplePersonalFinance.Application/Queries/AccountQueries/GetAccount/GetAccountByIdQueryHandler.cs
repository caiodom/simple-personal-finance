using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccount;

public class GetAccountByIdQueryHandler(IUnitOfWork uow, ICurrentUser currentUser) : IRequestHandler<GetAccountByIdQuery, ResultViewModel<AccountViewModel>>
{
    public async Task<ResultViewModel<AccountViewModel>> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetByIdAsync(request.Id);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.Id);

        return ResultViewModel<AccountViewModel>.Success(AccountViewModel.MapToViewModel(account));
    }
}
