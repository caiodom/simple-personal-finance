using SimplePersonalFinance.Core.Entities.Base;
using SimplePersonalFinance.Core.Enums;

namespace SimplePersonalFinance.Core.Entities;

public class Transaction:Entity
{
    public Guid AccountId { get; private set; }
    public int CategoryId { get; set; }
    public int TransactionTypeId { get; private set; }
    public string Description { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }


    public Transaction(Guid accountId, CategoryEnum category, TransactionTypeEnum transactionType, string description, decimal amount, DateTime date)
    {
        AccountId = accountId;
        CategoryId = (int)category;
        TransactionTypeId = (int)transactionType;
        Description = description;
        Amount = amount;
        Date = date;
    }




    //Ef Rel
    public Account Account { get;}
    public Category Category { get;}
}
