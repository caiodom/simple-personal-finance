using MediatR;
using SimplePersonalFinance.Application.ViewModels;

namespace SimplePersonalFinance.Application.Commands.RemoveAccount;

public class RemoveAccountCommand:IRequest<ResultViewModel<Guid>>
{
    public Guid Id { get; }
    public RemoveAccountCommand(Guid id)
    {
        Id = id;
    }
}
