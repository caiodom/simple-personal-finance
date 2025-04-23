using MediatR;
using SimplePersonalFinance.Application.ViewModels;

namespace SimplePersonalFinance.Application.Commands.AccountCommands.RemoveTransaction;

public class DeleteAccountTransactionCommand:IRequest<ResultViewModel<Guid>>
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    
    public DeleteAccountTransactionCommand(Guid id, Guid accountId)
    {
        Id = id;
        AccountId = accountId;
    }
}
