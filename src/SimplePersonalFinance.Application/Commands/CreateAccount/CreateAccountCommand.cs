using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Application.Commands.CreateAccount;

public class CreateAccountCommand:IRequest<ResultViewModel<Guid>>
{
    public Guid UserId { get; private set; }
    public AccountTypeEnum AccountType { get; private set; }
    public string Name { get; private set; }
    public decimal InitialBalance { get; private set; }

    public CreateAccountCommand(Guid userId, AccountTypeEnum accountType, string name, decimal initialBalance)
    {
        UserId = userId;
        AccountType = accountType;
        Name = name;
        InitialBalance = initialBalance;
    }
    public Account MapToEntity()
        => new(UserId,
               AccountType,
               Name,
               InitialBalance);

}
