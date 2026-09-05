namespace SimplePersonalFinance.Core.Domain.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public ICollection<Transaction> Transactions { get; } = new List<Transaction>();
    public ICollection<Budget> Budgets { get; } = new List<Budget>();

    public Category(int id, string name)
    {
        Id = id;
        Name = name;
    }

    // Constructor for EF Core
    protected Category()
    {
    }
}
