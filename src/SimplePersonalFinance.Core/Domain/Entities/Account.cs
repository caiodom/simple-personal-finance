using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Core.Domain.Entities;

public class Account:AggregateRoot
{
    public Guid UserId { get; private set; }
    public int AccountTypeId { get; private set; }
    public string Name { get;  private set; }
    public decimal  InitialBalance { get; private set; }
    public decimal CurrentBalance { get; private set; }


    private readonly List<Transaction> _transactions=[];
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

        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, category));

        return transaction;
    }

    public void EditTransaction(Guid transactionId, decimal newAmount, string newDescription, CategoryEnum category, TransactionTypeEnum transactionType)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found in this account");

        if (string.IsNullOrWhiteSpace(newDescription))
            throw new DomainException("Transaction description cannot be empty");

        decimal amountDifference = newAmount - transaction.Amount;
        transaction.UpdateDetails(newAmount, newDescription,category,transactionType);
        UpdateCurrentBalance(amountDifference, transactionType != TransactionTypeEnum.EXPENSE);

        AddDomainEvent(new BudgetEvaluationRequestedDomainEvent(Id, category));
    }

    public void DeleteTransaction(Guid transactionId)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found in this account");


        transaction.SetAsDeleted();
        UpdateCurrentBalance(transaction.Amount, transaction.TransactionTypeId == (int)TransactionTypeEnum.EXPENSE);
    }

    public void UpdateCurrentBalance(decimal amount, bool isAddition)
    {
        if (isAddition)
            CurrentBalance += amount;
        else
            CurrentBalance -= amount;
    }



    // Constructor for EF Core
    protected Account() { }






    //Ef Rel
    public AccountType AccountType { get; }
    public User User { get;}
}
