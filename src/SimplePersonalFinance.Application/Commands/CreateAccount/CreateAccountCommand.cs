using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Application.Commands.CreateAccount;

public class CreateAccountCommand:IRequest<ResultViewModel<Guid>>
{
    public Guid UserId { get; private set; }
    public AccountTypeEnum AccountType { get; private set; }
    public string Name { get; private set; }
    public decimal InitialBalance { get; private set; }

    public CreateAccountCommand(AccountTypeEnum accountType, string name, decimal initialBalance)
    {
        AccountType = accountType;
        Name = name;
        InitialBalance = initialBalance;
    }

    public void SetUserId(Guid userId)
    {
        this.UserId = userId;
    }
    public Account MapToEntity()
        => new(UserId,
               AccountType,
               Name,
               InitialBalance);

}
