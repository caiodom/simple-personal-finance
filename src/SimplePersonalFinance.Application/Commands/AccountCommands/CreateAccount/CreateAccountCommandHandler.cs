using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Results;
using SimplePersonalFinance.Core.Domain.ValueObjects;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.CreateAccount;

public class CreateAccountCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateAccountCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = request.MapToEntity();

        await uow.Accounts.AddAsync(account);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(account.Id);
    }
}
