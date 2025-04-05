namespace SimplePersonalFinance.Core.Entities;

public class Category
{
    public int Id { get; private set; }
    public string Name { get; private set; }

    public Category(int id,string name)
    {
        Id=id;
        Name=name;
    }

    //Ef Rel
    public ICollection<Transaction> Transactions { get; }
    public ICollection<Budget> Budgets { get; }
}
