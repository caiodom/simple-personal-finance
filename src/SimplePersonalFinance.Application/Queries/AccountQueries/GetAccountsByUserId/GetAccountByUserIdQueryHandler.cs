using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountsByUserId;

public class GetAccountByUserIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAccountByUserIdQuery, ResultViewModel<PaginatedResult<AccountViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<AccountViewModel>>> Handle(GetAccountByUserIdQuery request, CancellationToken cancellationToken)
    {
        var (accounts, totalItems) = await uow.Accounts.GetAccountsByUserIdAsync(
            request.UserId,
            request.PageNumber,
            request.PageSize);

        var items = accounts
            .Select(AccountViewModel.MapToViewModel)
            .ToList();

        var result = new PaginatedResult<AccountViewModel>(items, totalItems, request.PageNumber, request.PageSize);
        return ResultViewModel<PaginatedResult<AccountViewModel>>.Success(result);
    }
}
