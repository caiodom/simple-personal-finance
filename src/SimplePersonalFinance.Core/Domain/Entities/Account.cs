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
    public decimal InitialBalance { get; private set; }
    public decimal CurrentBalance { get; private set; }


    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public Account(Guid userId, AccountTypeEnum accountTypeId, string name, decimal initialBalance)
    {
        UserId = userId;
        AccountTypeId = (int)accountTypeId;
        Name = name;
        InitialBalance = initialBalance;
        CurrentBalance = initialBalance;
    }

    public Transaction AddTransaction(string description, decimal amount, CategoryEnum category, TransactionTypeEnum transactionType, DateTime date)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Transaction description cannot be empty");

        var transaction = new Transaction(Id, category, transactionType, description, amount, date);
        _transactions.Add(transaction);



        UpdateCurrentBalance(amount, transactionType != TransactionTypeEnum.EXPENSE);

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

        var originalValue = new MoneyAmount(transaction.Amount, transaction.TransactionTypeId == (int)TransactionTypeEnum.INCOME);
        var newValue = new MoneyAmount(newAmount, transactionType == TransactionTypeEnum.INCOME);

        var balanceUpdater= CreateBalanceUpdateStrategy(transaction, transactionType);
        balanceUpdater.UpdateBalance(this, originalValue, newValue);


        #region >>old way<<
        //UpdateCurrentBalance(amountDifference, transactionType != TransactionTypeEnum.EXPENSE);


        /* if (transaction.TransactionTypeId != (int)transactionType)
         {
             UpdateCurrentBalance(transaction.Amount, ReverseTransaction(transaction.TransactionTypeId));
             UpdateCurrentBalance(transaction.Amount, transactionType != TransactionTypeEnum.EXPENSE);
         }
         else
         {
             decimal ammountDifference = 0;
             if (transaction.Amount > newAmount)
                 ammountDifference = transaction.Amount - newAmount;
             else
                 ammountDifference = newAmount - transaction.Amount;

                 UpdateCurrentBalance(ammountDifference, transactionType != TransactionTypeEnum.EXPENSE);
         }*/
        #endregion


        transaction.UpdateDetails(newAmount, newDescription, category, transactionType);
        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, UserId, category));
    }

    public void DeleteTransaction(Guid transactionId)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found in this account");

        transaction.SetAsDeleted();
        UpdateCurrentBalance(transaction.Amount, transaction.TransactionTypeId == (int)TransactionTypeEnum.EXPENSE);
    }


    public void UpdateCurrentBalance(decimal amount, bool isAdding)
    {
        if (isAdding)
            CurrentBalance += amount;
        else
            CurrentBalance -= amount;

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

    //Ef Rel
    public AccountType AccountType { get; }
    public User User { get; }
}
