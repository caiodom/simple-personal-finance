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
        ValidateAccountName(name);

        UserId = userId;
        AccountTypeId = (int)accountTypeId;
        Name = name;

        InitializeBalances(initialBalance);
    }

    public Transaction AddTransaction(
    string description,
    decimal amount,
    CategoryEnum category,
    TransactionTypeEnum transactionType,
    DateTime date)
    {
        ValidateTransactionData(description, amount);

        var transaction = CreateTransaction(category, transactionType, description, amount, date);
        _transactions.Add(transaction);

        var money = CreateMoney(amount);
        UpdateBalanceForNewTransaction(money, transactionType);
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
        ValidateTransactionData(newDescription, newAmount);

        var transaction = FindTransactionById(transactionId);
        var currentType = ExtractTransactionType(transaction);
        var originalValue = ExtractMoneyFromTransaction(transaction);
        var newValue = CreateMoney(newAmount);

        UpdateBalanceForEditedTransaction(transaction, originalValue, newValue, currentType, transactionType);
        transaction.UpdateDetails(newAmount, newDescription, category, transactionType);
        PublishBudgetEvaluationEvent(category);
    }

    public void UpdateName(string newName)
    {
        ValidateAccountName(newName);
        Name = newName;
    }

    public void DeleteAccount()
    {
        RemoveAllTransactions();
        SetAsDeleted();
    }

    public void DeleteTransaction(Guid transactionId)
    {
        var transaction = FindTransactionById(transactionId);
        RemoveAccountTransactions(transaction);
    }

    public void UpdateCurrentBalance(Money amount)
    {
        ArgumentNullException.ThrowIfNull(amount, nameof(amount));
        CurrentBalance = CurrentBalance.Add(amount);
    }

    private void RemoveAllTransactions()
    {
        foreach (var transaction in _transactions)
            RemoveAccountTransactions(transaction);
    }

    private void ValidateAccountName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Account name cannot be empty");
    }

    private void InitializeBalances(decimal initialBalance)
    {
        var initialBalanceResult = CreateMoney(initialBalance);
        InitialBalance = initialBalanceResult;
        CurrentBalance = CreateMoney(initialBalance);
    }

    private Money CreateMoney(decimal amount)
    {
        var moneyResult = Money.Create(amount);
        if (moneyResult.IsFailure)
            throw new DomainException(moneyResult.Error);

        return moneyResult.Value;
    }

    private void UpdateBalanceForNewTransaction(Money money, TransactionTypeEnum transactionType)
    {
        if (IsExpenseType(transactionType))
            DecreaseCurrencyBalance(money);

        if (IsIncomeType(transactionType))
            IncreaseCurrencyBalance(money);
    }

    private void DecreaseCurrencyBalance(Money money)
    {
        UpdateCurrentBalance(money.Scale(-1));
    }

    private void IncreaseCurrencyBalance(Money money)
    {
        UpdateCurrentBalance(money);
    }

    private bool IsExpenseType(TransactionTypeEnum transactionType)
    {
        return transactionType == TransactionTypeEnum.EXPENSE;
    }

    private bool IsIncomeType(TransactionTypeEnum transactionType)
    {
        return transactionType == TransactionTypeEnum.INCOME;
    }

    private bool IsIncomeType(int transactionTypeId)
    {
        return transactionTypeId == (int)TransactionTypeEnum.INCOME;
    }

    private bool IsExpenseType(int transactionTypeId)
    {
        return transactionTypeId == (int)TransactionTypeEnum.EXPENSE;
    }

    private TransactionTypeEnum ExtractTransactionType(Transaction transaction)
    {
        return (TransactionTypeEnum)transaction.TransactionTypeId;
    }

    private Money ExtractMoneyFromTransaction(Transaction transaction)
    {
        return CreateMoney(transaction.Amount);
    }

    private void ValidateTransactionData(string description, decimal amount)
    {
        ValidateTransactionDescription(description);
        ValidateTransactionAmount(amount);
    }

    private void ValidateTransactionDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Transaction description cannot be empty");
    }

    private void ValidateTransactionAmount(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("Transaction amount cannot be negative");
    }

    private Transaction CreateTransaction(
        CategoryEnum category,
        TransactionTypeEnum transactionType,
        string description,
        decimal amount,
        DateTime date)
    {
        return new Transaction(Id, category, transactionType, description, amount, date);
    }

    private Transaction FindTransactionById(Guid transactionId)
    {
        var transaction = FindTransaction(transactionId);
        ValidateTransactionExists(transaction, transactionId);
        return transaction;
    }

    private Transaction FindTransaction(Guid transactionId)
    {
        return _transactions.FirstOrDefault(transaction => transaction.Id == transactionId);
    }

    private void ValidateTransactionExists(Transaction transaction, Guid transactionId)
    {
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found in this account");
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

    private void RemoveAccountTransactions(Transaction transaction)
    {
        transaction.SetAsDeleted();
        var money = ExtractMoneyFromTransaction(transaction);
        AdjustBalanceForRemovedTransaction(money, transaction.TransactionTypeId);
    }

    private void AdjustBalanceForRemovedTransaction(Money money, int transactionTypeId)
    {
        if (IsIncomeType(transactionTypeId))
            DecreaseCurrencyBalance(money);

        if (IsExpenseType(transactionTypeId))
            IncreaseCurrencyBalance(money);
    }

    private IBalanceUpdateStrategy CreateBalanceUpdateStrategy(
        Transaction transaction,
        TransactionTypeEnum newTransactionType)
    {
        var currentType = ExtractTransactionType(transaction);
        if (IsTransactionTypeChanged(currentType, newTransactionType))
            return new TransactionTypeChangeStrategy();

        return new SameTransactionTypeStrategy();
    }

    private bool IsTransactionTypeChanged(TransactionTypeEnum currentType, TransactionTypeEnum newType)
    {
        return currentType != newType;
    }

    private void PublishBudgetEvaluationEvent(CategoryEnum category)
    {
        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, UserId, category));
    }

    // Constructor for EF Core
    protected Account() { }

    // EF Relationships
    public AccountType AccountType { get; }
    public User User { get; }
}
