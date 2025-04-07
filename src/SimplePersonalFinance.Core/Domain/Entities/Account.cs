using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class Account:Entity
{
    public Guid UserId { get; private set; }
    public int AccountTypeId { get; private set; }

    public string Name { get;  private set; }
    public decimal  InitialBalance { get; private set; }
    public decimal CurrentBalance { get; private set; }

    public Account(Guid userId, AccountTypeEnum accountTypeId, string name, decimal initialBalance, decimal currentBalance)
    {
        UserId = userId;
        AccountTypeId = (int)accountTypeId;
        Name = name;
        InitialBalance = initialBalance;
        CurrentBalance = currentBalance;
    }

    // Constructor for EF Core
    protected Account() { }






    //Ef Rel
    public AccountType AccountType { get; }
    public ICollection<Transaction> Transactions { get; }
    public User User { get;}
}
