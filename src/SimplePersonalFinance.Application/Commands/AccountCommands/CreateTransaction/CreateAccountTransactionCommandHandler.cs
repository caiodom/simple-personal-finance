using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Interfaces.Data;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.CreateTransaction;

public class CreateAccountTransactionCommandHandler(IUnitOfWork uow, ICurrentUser currentUser):IRequestHandler<CreateAccountTransactionCommand, ResultViewModel<Guid>>
{
    public async Task<ResultViewModel<Guid>> Handle(CreateAccountTransactionCommand request, CancellationToken cancellationToken)
    {
        var account = await uow.Accounts.GetByIdAsync(request.AccountId);

        if (account is null || account.UserId != currentUser.UserId)
            throw new EntityNotFoundException("Account", request.AccountId);

        var transaction = account.AddTransaction(request.Description, request.Amount, request.CategoryId, request.TransactionTypeId, request.Date);
        uow.Accounts.AddAccountTransaction(transaction);
        await uow.SaveChangesAsync();

        return ResultViewModel<Guid>.Success(transaction.Id, "Transaction created successfully");
    }
}
