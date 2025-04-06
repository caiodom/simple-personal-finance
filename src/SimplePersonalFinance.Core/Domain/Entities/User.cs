using SimplePersonalFinance.Core.Domain.Entities.Base;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class User:Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }

    public User(string name, string email, string passwordHash)
    {
        Name=name;
        Email=email;
        PasswordHash=passwordHash;

    }

    //Ef Rel
    public ICollection<Account> Accounts { get; }
    public ICollection<Budget> Budgets { get;}
}
