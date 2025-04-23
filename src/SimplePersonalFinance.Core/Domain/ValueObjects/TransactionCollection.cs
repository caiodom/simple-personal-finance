using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Exceptions;

namespace SimplePersonalFinance.Core.Domain.ValueObjects;

public class TransactionCollection
{
    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public Transaction Add(TransactionDetails details)
    {
        ValidateTransactionDetails(details);

        var transaction = new Transaction(
            details.AccountId,
            details.Category,
            details.TransactionType,
            details.Description,
            details.Amount,
            details.Date);

        _transactions.Add(transaction);
        return transaction;
    }

    public void Update(Guid transactionId, TransactionDetails details)
    {
        ValidateTransactionDetails(details);
        var transaction = GetById(transactionId);

        transaction.UpdateDetails(
            details.Amount,
            details.Description,
            details.Category,
            details.TransactionType);
    }

    public Transaction GetById(Guid transactionId)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == transactionId && t.IsActive);
        if (transaction == null)
            throw new DomainException($"Transaction with id {transactionId} not found");

        return transaction;
    }

    public void Remove(Guid transactionId)
    {
        var transaction = GetById(transactionId);
        transaction.SetAsDeleted();
    }

    public void ForEach(Action<Transaction> action)
    {
        foreach (var transaction in _transactions.Where(t => t.IsActive))
            action(transaction);
    }

    public void Clear()
    {
        foreach (var transaction in _transactions.Where(t => t.IsActive))
            transaction.SetAsDeleted();
    }


    private static void ValidateTransactionDetails(TransactionDetails details)
    {
        if (string.IsNullOrWhiteSpace(details.Description))
            throw new DomainException("Transaction description cannot be empty");

        if (details.Amount < 0)
            throw new DomainException("Transaction amount cannot be negative");
    }
}
