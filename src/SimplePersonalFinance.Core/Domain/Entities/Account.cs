using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class Account : AggregateRoot
{
    public Guid UserId { get; private set; }
    public int AccountTypeId { get; private set; }
    public string Name { get; private set; }
    public decimal InitialBalance { get; private set; }
    public decimal CurrentBalance { get; private set; }

    private readonly TransactionCollection _transactions;
    public IReadOnlyCollection<Transaction> Transactions => _transactions.Transactions;

    public Account(Guid userId, AccountTypeEnum accountTypeId, string name, decimal initialBalance)
    {
        ValidateAccountName(name);
        ValidateInitialBalance(initialBalance);

        UserId = userId;
        AccountTypeId = (int)accountTypeId;
        Name = name;
        InitialBalance = initialBalance;
        CurrentBalance = initialBalance;
        _transactions = new TransactionCollection();
    }

    public Transaction AddTransaction(
        string description,
        decimal amount,
        CategoryEnum category,
        TransactionTypeEnum transactionType,
        DateTime date)
    {
        var transactionDetails = TransactionDetails.Create(Id, description, amount, category, transactionType, date);
        var transaction = _transactions.Add(transactionDetails);

        ApplyTransactionEffect(amount, transactionType);
        PublishBudgetEvaluationEvent(category);

        return transaction;
    }

    public void EditTransaction(
        Guid transactionId,
        decimal newAmount,
        string newDescription,
        CategoryEnum category,
        TransactionTypeEnum transactionType)
    {
        var transaction = _transactions.GetById(transactionId);
        var originalAmount = transaction.Amount;
        var originalType = (TransactionTypeEnum)transaction.TransactionTypeId;

        var transactionDetails = TransactionDetails.Create(Id, newDescription, newAmount, category, transactionType, transaction.Date);
        _transactions.Update(transactionId, transactionDetails);

        RevertTransactionEffect(originalAmount, originalType);
        ApplyTransactionEffect(newAmount, transactionType);
        PublishBudgetEvaluationEvent(category);
    }

    public void UpdateName(string newName)
    {
        ValidateAccountName(newName);
        Name = newName;
    }

    public void DeleteAccount()
    {
        _transactions.ForEach(transaction =>
            RevertTransactionEffect(transaction.Amount, (TransactionTypeEnum)transaction.TransactionTypeId));

        _transactions.Clear();
        SetAsDeleted();
    }

    public void DeleteTransaction(Guid transactionId)
    {
        var transaction = _transactions.GetById(transactionId);

        RevertTransactionEffect(transaction.Amount, (TransactionTypeEnum)transaction.TransactionTypeId);
        _transactions.Remove(transactionId);
    }

    private void ApplyTransactionEffect(decimal amount, TransactionTypeEnum transactionType)
    {
        CurrentBalance += GetSignedAmount(amount, transactionType);
    }

    private void RevertTransactionEffect(decimal amount, TransactionTypeEnum transactionType)
    {
        CurrentBalance -= GetSignedAmount(amount, transactionType);
    }

    private static decimal GetSignedAmount(decimal amount, TransactionTypeEnum transactionType)
    {
        return transactionType switch
        {
            TransactionTypeEnum.INCOME => amount,
            TransactionTypeEnum.EXPENSE => -amount,
            _ => throw new DomainException("Transaction type is invalid")
        };
    }

    private static void ValidateAccountName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Account name cannot be empty");
    }

    private static void ValidateInitialBalance(decimal initialBalance)
    {
        if (initialBalance < 0)
            throw new DomainException("Initial balance cannot be negative");
    }

    private void PublishBudgetEvaluationEvent(CategoryEnum category)
    {
        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, UserId, category));
    }

    // Constructor for EF Core
    protected Account()
    {
        _transactions = new TransactionCollection();
    }

    // EF relationships
    public AccountType AccountType { get; }
    public User User { get; }
}
