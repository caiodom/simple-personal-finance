using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Application.ViewModels.Accounts;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Shared.Contracts;
using SimplePersonalFinance.Shared.Extensions;

namespace SimplePersonalFinance.Application.Queries.AccountQueries.GetAccountsByUserId;

public class GetAccountByUserIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAccountByUserIdQuery, ResultViewModel<PaginatedResult<AccountViewModel>>>
{
    public async Task<ResultViewModel<PaginatedResult<AccountViewModel>>> Handle(GetAccountByUserIdQuery request, CancellationToken cancellationToken)
    {
        var accounts = uow.Accounts.GetAccountsByUserIdAsync(request.UserId);

        if (accounts == null)
            throw new EntityNotFoundException("Account", request.UserId);

        var results = await accounts.Select(a => AccountViewModel.MapToViewModel(a))
                                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize,
                                        cancellationToken);

        return ResultViewModel<PaginatedResult<AccountViewModel>>.Success(results);

    }
}
