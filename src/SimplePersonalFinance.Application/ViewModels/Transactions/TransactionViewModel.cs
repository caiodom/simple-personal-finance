using SimplePersonalFinance.Core.Domain.Entities;

namespace SimplePersonalFinance.Application.ViewModels.Accounts;

public class TransactionViewModel
{
    public Guid AccountId { get; private set; }
    public int CategoryId { get; private set; }
    public string CategoryName { get; private set; }
    public int TransactionTypeId { get; private set; }
    public string TransactionTypeName { get; private set; }
    public string Description { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }


    public TransactionViewModel(Guid accountId, int categoryId, string categoryName, int transactionTypeId, 
                                string transactionTypeName, string description, decimal amount, DateTime date)
    {
        AccountId = accountId;
        CategoryId = categoryId;
        CategoryName = categoryName;
        TransactionTypeId = transactionTypeId;
        TransactionTypeName = transactionTypeName;
        Description = description;
        Amount = amount;
        Date = date;
    }

    public static TransactionViewModel ToViewModel(Transaction transaction)
    {
       return new (transaction.AccountId,
                                 transaction.CategoryId,
                                 transaction.Category.Name,
                                 transaction.TransactionTypeId,
                                 transaction.TransactionType.Name,
                                 transaction.Description,
                                 transaction.Amount,
                                 transaction.Date);
    }
}
