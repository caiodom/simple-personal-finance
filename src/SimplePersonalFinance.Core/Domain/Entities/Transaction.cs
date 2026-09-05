using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class Transaction : Entity
{
    public Guid AccountId { get; private set; }
    public int CategoryId { get; private set; }
    public int TransactionTypeId { get; private set; }
    public string Description { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }

    public Transaction(
        Guid accountId,
        CategoryEnum category,
        TransactionTypeEnum transactionType,
        string description,
        decimal amount,
        DateTime date)
    {
        ValidateAccountId(accountId);
        ValidateDetails(description, amount, category, transactionType);
        ValidateDate(date);

        AccountId = accountId;
        CategoryId = (int)category;
        TransactionTypeId = (int)transactionType;
        Description = description;
        Amount = amount;
        Date = date;
    }

    public void UpdateDetails(
        decimal newAmount,
        string newDescription,
        CategoryEnum newCategory,
        TransactionTypeEnum newTransactionType)
    {
        ValidateDetails(newDescription, newAmount, newCategory, newTransactionType);

        CategoryId = (int)newCategory;
        TransactionTypeId = (int)newTransactionType;
        Amount = newAmount;
        Description = newDescription;
    }

    private static void ValidateAccountId(Guid accountId)
    {
        if (accountId == Guid.Empty)
            throw new DomainException("Transaction account id cannot be empty");
    }

    private static void ValidateDetails(
        string description,
        decimal amount,
        CategoryEnum category,
        TransactionTypeEnum transactionType)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Transaction description cannot be empty");

        if (amount < 0)
            throw new DomainException("Transaction amount cannot be negative");

        if (!Enum.IsDefined(category))
            throw new DomainException("Transaction category is invalid");

        if (!Enum.IsDefined(transactionType))
            throw new DomainException("Transaction type is invalid");
    }

    private static void ValidateDate(DateTime date)
    {
        if (date == default)
            throw new DomainException("Transaction date must be provided");
    }

    // Constructor for EF Core
    protected Transaction() { }

    // EF relationships
    public Account Account { get; }
    public Category Category { get; }
    public TransactionType TransactionType { get; }
}
