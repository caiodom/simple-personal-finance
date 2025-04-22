using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Application.ViewModels.Accounts;

public class AccountViewModel
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int AccountTypeId { get; private set; }
    public string AccountTypeName { get; private set; }
    public string Name { get; private set; }
    public decimal InitialBalance { get; private set; }
    public decimal CurrentBalance { get; private set; }

    public AccountViewModel(Guid id, Guid userId, int accountTypeId, string name, string accountTypeName, decimal initialBalance, decimal currentBalance)
    {
        Id = id;
        UserId = userId;
        AccountTypeId = accountTypeId;
        AccountTypeName = accountTypeName;
        Name = name;
        InitialBalance = initialBalance;
        CurrentBalance = currentBalance;
    }

    public static AccountViewModel MapToViewModel(Account account)
    {
        return new(account.Id,
                   account.UserId,
                   account.AccountTypeId,
                   account.Name,
                   account.AccountType.Name,
                   account.InitialBalance.Amount,
                   account.CurrentBalance.Amount);
    }

}
