namespace SimplePersonalFinance.Core.Entities;

public class AccountType
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public AccountType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    //Ef rels
    public ICollection<Account> Accounts { get; }
}
