using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Core.Domain.ValueObjects;

public class TransactionDetails
{
    public Guid AccountId { get; }
    public string Description { get; }
    public decimal Amount { get; }
    public CategoryEnum Category { get; }
    public TransactionTypeEnum TransactionType { get; }
    public DateTime Date { get; }

    private TransactionDetails(
        Guid accountId,
        string description,
        decimal amount,
        CategoryEnum category,
        TransactionTypeEnum transactionType,
        DateTime date)
    {
        AccountId = accountId;
        Description = description;
        Amount = amount;
        Category = category;
        TransactionType = transactionType;
        Date = date;
    }

    public static TransactionDetails Create(
       Guid accountId,
       string description,
       decimal amount,
       CategoryEnum category,
       TransactionTypeEnum transactionType,
       DateTime date)
    {
        return new TransactionDetails(accountId, description, amount, category, transactionType, date);
    }
}
