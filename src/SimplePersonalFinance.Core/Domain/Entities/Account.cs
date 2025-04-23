using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Domain.Exceptions;
using SimplePersonalFinance.Core.Domain.Strategies.BalanceUpdate;
using SimplePersonalFinance.Core.Domain.ValueObjects;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class Account : AggregateRoot
{
    public Guid UserId { get; private set; }
    public int AccountTypeId { get; private set; }
    public string Name { get; private set; }
    public Money InitialBalance { get; private set; }
    public Money CurrentBalance { get; private set; }

    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public Account(Guid userId, AccountTypeEnum accountTypeId, string name, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Account name cannot be empty");

        UserId = userId;
        AccountTypeId = (int)accountTypeId;
        Name = name;

        var initialBalanceResult = Money.Create(initialBalance);
        if (initialBalanceResult.IsFailure)
            throw new DomainException(initialBalanceResult.Error);

        InitialBalance = initialBalanceResult.Value;
        CurrentBalance = Money.Create(initialBalance).Value;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new DomainException("Account name cannot be empty");

        Name = newName;
    }
    public void DeleteAccount()
    {
        foreach (var transaction in _transactions)
            RemoveAccountTransactions(transaction);

        SetAsDeleted();
    }

    public Transaction AddTransaction(string description, decimal amount, CategoryEnum category,
        TransactionTypeEnum transactionType, DateTime date)
    {
        ValidateTransactionData(description, amount);

        var transaction = CreateTransaction(category, transactionType, description, amount, date);
        _transactions.Add(transaction);

        var moneyResult = Money.Create(amount);
        if (moneyResult.IsFailure)
            throw new DomainException(moneyResult.Error);

        UpdateBalance(moneyResult.Value, transactionType);
        PublishBudgetEvaluationEvent(category);

        return transaction;
    }

    public void EditTransaction(Guid transactionId, decimal newAmount, string newDescription,
        CategoryEnum category, TransactionTypeEnum transactionType)
    {
        ValidateTransactionData(newDescription, newAmount);

        var transaction = FindTransactionById(transactionId);

        var currentTransactionType = (TransactionTypeEnum)transaction.TransactionTypeId;
        var originalValueResult = Money.Create(transaction.Amount);
        var newValueResult = Money.Create(newAmount);

        if (originalValueResult.IsFailure)
            throw new DomainException(originalValueResult.Error);
        if (newValueResult.IsFailure)
            throw new DomainException(newValueResult.Error);

        UpdateBalanceForEditedTransaction(
            transaction,
            originalValueResult.Value,
            newValueResult.Value,
            currentTransactionType,
            transactionType);

        transaction.UpdateDetails(newAmount, newDescription, category, transactionType);
        PublishBudgetEvaluationEvent(category);
    }

  

    public void DeleteTransaction(Guid transactionId)
    {
        var transaction = FindTransactionById(transactionId);
        RemoveAccountTransactions(transaction);
    }

    

    public void UpdateCurrentBalance(Money amount)
    {
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));

        CurrentBalance = CurrentBalance.Add(amount);
    }


    #region Private Helper Methods

    private void ValidateTransactionData(string description, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Transaction description cannot be empty");

        if (amount < 0)
            throw new DomainException("Transaction amount cannot be negative");
    }

    private Transaction CreateTransaction(CategoryEnum category, TransactionTypeEnum transactionType,
        string description, decimal amount, DateTime date)
    {
        return new Transaction(Id, category, transactionType, description, amount, date);
    }

    private Transaction FindTransactionById(Guid transactionId)
    {
        var transaction = _transactions.FirstOrDefault(transaction => transaction.Id == transactionId);
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found in this account");

        return transaction;
    }

    private void UpdateBalanceForEditedTransaction(
        Transaction transaction,
        Money originalValue,
        Money newValue,
        TransactionTypeEnum currentType,
        TransactionTypeEnum newType)
    {
        var balanceUpdater = CreateBalanceUpdateStrategy(transaction, newType);
        balanceUpdater.UpdateBalance(this, originalValue, newValue, currentType, newType);
    }

    private void UpdateBalance(Money money, TransactionTypeEnum transactionType)
    {
        if (transactionType == TransactionTypeEnum.EXPENSE)
            UpdateCurrentBalance(money.Scale(-1));
        else
            UpdateCurrentBalance(money);
    }

    private void RemoveAccountTransactions(Transaction transaction)
    {
        transaction.SetAsDeleted();

        var moneyResult = Money.Create(transaction.Amount);
        if (moneyResult.IsFailure)
            throw new DomainException(moneyResult.Error);

        var money = moneyResult.Value;

        if (transaction.TransactionTypeId == (int)TransactionTypeEnum.INCOME)
            UpdateCurrentBalance(money.Scale(-1));
        else
            UpdateCurrentBalance(money);
    }

    private IBalanceUpdateStrategy CreateBalanceUpdateStrategy(Transaction transaction, TransactionTypeEnum newTransactionType)
    {
        var currentTransactionType = (TransactionTypeEnum)transaction.TransactionTypeId;
        if (currentTransactionType != newTransactionType)
            return new TransactionTypeChangeStrategy();

        return new SameTransactionTypeStrategy();
    }

    private void PublishBudgetEvaluationEvent(CategoryEnum category)
    {
        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, UserId, category));
    }

    #endregion

    // Constructor for EF Core
    protected Account() { }

    // EF Relationships
    public AccountType AccountType { get; }
    public User User { get; }
}
