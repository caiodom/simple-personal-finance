namespace SimplePersonalFinance.Core.Domain.Entities;

public class TransactionType
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public bool IsCredit { get; private set; }

    public TransactionType(int id, string name, bool isCredit)
    {
        Id = id;
        Name = name;
        IsCredit = isCredit;
    }

    public ICollection<Transaction> Transactions { get; }

    // Constructor for EF Core
    protected TransactionType() { }
}
