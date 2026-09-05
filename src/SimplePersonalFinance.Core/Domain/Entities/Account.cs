using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Domain.Services;
using SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;
using SimplePersonalFinance.Core.Domain.ValueObjects;
using SimplePersonalFinance.Core.Interfaces.Domain.Services;
using SimplePersonalFinance.Core.Interfaces.Domain.Strategies;

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

        var balanceManager = CreateBalanceManager();
        balanceManager.ApplyNewTransaction(this, amount, transactionType);

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

        var balanceManager = CreateBalanceManager();
        var balanceUpdateStrategy = CreateUpdateStrategyFactory().Create(originalType, transactionType);

        balanceManager.UpdateBalanceForEdit(
            this,
            originalAmount,
            newAmount,
            originalType,
            transactionType,
            balanceUpdateStrategy);

        PublishBudgetEvaluationEvent(category);
    }

    public void UpdateName(string newName)
    {
        ValidateAccountName(newName);
        Name = newName;
    }

    public void DeleteAccount()
    {
        var balanceManager = CreateBalanceManager();
        _transactions.ForEach(transaction =>
            balanceManager.RevertTransaction(this, transaction)
        );

        _transactions.Clear();
        SetAsDeleted();
    }

    public void DeleteTransaction(Guid transactionId)
    {
        var transaction = _transactions.GetById(transactionId);
        var balanceManager = CreateBalanceManager();

        balanceManager.RevertTransaction(this, transaction);
        _transactions.Remove(transactionId);
    }

    public void UpdateCurrentBalance(decimal amount)
    {
        CurrentBalance += amount;
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

    private static IBalanceManager CreateBalanceManager() => new BalanceManager();

    private static IBalanceUpdateStrategyFactory CreateUpdateStrategyFactory() => new BalanceUpdateStrategyFactory();

    // Constructor for EF Core
    protected Account()
    {
        _transactions = new TransactionCollection();
    }

    // EF relationships
    public AccountType AccountType { get; }
    public User User { get; }
}
