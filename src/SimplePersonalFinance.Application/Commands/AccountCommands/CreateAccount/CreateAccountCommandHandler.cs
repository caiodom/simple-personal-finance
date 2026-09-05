using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.CreateAccount;

public class CreateAccountCommandHandler(
    IAccountRepository accounts,
    IUnitOfWork uow) : IRequestHandler<CreateAccountCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = request.MapToEntity();

        await accounts.AddAsync(account);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(account.Id, "Account created successfully");
    }
}
