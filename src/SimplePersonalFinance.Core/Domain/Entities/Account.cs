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
        InitialBalance = Money.Create(initialBalance).Value;
        CurrentBalance = Money.Create(initialBalance).Value;
    }

    public Transaction AddTransaction(string description, decimal amount, CategoryEnum category, TransactionTypeEnum transactionType, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Transaction description cannot be empty");

        var transaction = new Transaction(Id, category, transactionType, description, amount, date);
        _transactions.Add(transaction);

        // Create Money object and update balance based on transaction type
        var moneyResult = Money.Create(amount);
        if (moneyResult.IsFailure)
            throw new DomainException(moneyResult.Error);

        var money = moneyResult.Value;

        // Aplica escala negativa para despesas
        if (transactionType == TransactionTypeEnum.EXPENSE)
            UpdateCurrentBalance(money.Scale(-1));
        else
            UpdateCurrentBalance(money);

        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, UserId, category));

        return transaction;
    }

    public void EditTransaction(Guid transactionId, decimal newAmount, string newDescription, CategoryEnum category, TransactionTypeEnum transactionType)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found in this account");

        if (string.IsNullOrWhiteSpace(newDescription))
            throw new DomainException("Transaction description cannot be empty");

        var currentTransactionType = (TransactionTypeEnum)transaction.TransactionTypeId;
        var originalValueResult = Money.Create(transaction.Amount);
        var newValueResult = Money.Create(newAmount);

        if (originalValueResult.IsFailure)
            throw new DomainException(originalValueResult.Error);
        if (newValueResult.IsFailure)
            throw new DomainException(newValueResult.Error);

        var balanceUpdater = CreateBalanceUpdateStrategy(transaction, transactionType);

        balanceUpdater.UpdateBalance(
            this,
            originalValueResult.Value,
            newValueResult.Value,
            currentTransactionType,
            transactionType);

        transaction.UpdateDetails(newAmount, newDescription, category, transactionType);
        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, UserId, category));
    }

    public void DeleteAccount()
    {
        foreach (var transaction in _transactions)
            RemoveAccountTransactions(transaction);

        SetAsDeleted();
    }

    public void DeleteTransaction(Guid transactionId)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found in this account");

        RemoveAccountTransactions(transaction);
    }

    public void UpdateCurrentBalance(Money amount)
    {
        if (amount == null)
            throw new ArgumentNullException(nameof(amount));

        CurrentBalance = CurrentBalance.Add(amount);
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

    // Constructor for EF Core
    protected Account() { }

    // EF Relationships
    public AccountType AccountType { get; }
    public User User { get; }
}
