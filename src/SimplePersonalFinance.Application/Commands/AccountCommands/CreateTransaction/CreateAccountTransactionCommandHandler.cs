using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.CreateTransaction;

public class CreateAccountTransactionCommandHandler(
    IAccountRepository accounts,
    IUnitOfWork uow,
    ICurrentUser currentUser) : IRequestHandler<CreateAccountTransactionCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(CreateAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await accounts.GetByIdAsync(request.AccountId, cancellationToken);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        var transaction = account.AddTransaction(request.Description, request.Amount, request.CategoryId, request.TransactionTypeId, request.Date);
        accounts.AddAccountTransaction(transaction);
        await uow.SaveChangesAsync(cancellationToken);

        return ResultViewModel<Guid>.Success(transaction.Id, "Transaction created successfully");
    }
}
