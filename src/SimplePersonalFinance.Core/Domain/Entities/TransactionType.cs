namespace SimplePersonalFinance.Core.Domain.Entities;

public class TransactionType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public bool IsCredit { get; private set; }

    public ICollection<Transaction> Transactions { get; } = new List<Transaction>();

    public TransactionType(int id, string name, bool isCredit)
    {
        Id = id;
        Name = name;
        IsCredit = isCredit;
    }

    // Constructor for EF Core
    protected TransactionType()
    {
    }
}
