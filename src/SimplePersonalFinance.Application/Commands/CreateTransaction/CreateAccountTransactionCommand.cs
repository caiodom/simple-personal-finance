using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;

namespace SimplePersonalFinance.Application.Commands.CreateTransaction;

public class CreateAccountTransactionCommand : IRequest<ResultViewModel<Guid>>
{
    public Guid AccountId { get; private set; }
    public CategoryEnum CategoryId { get; private set; }
    public TransactionTypeEnum TransactionTypeId { get; private set; }
    public string Description { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }

    public CreateAccountTransactionCommand(Guid accountId, CategoryEnum categoryId, TransactionTypeEnum transactionTypeId, string description, decimal amount, DateTime date)
    {
        AccountId = accountId;
        CategoryId = categoryId;
        TransactionTypeId = transactionTypeId;
        Description = description;
        Amount = amount;
        Date = date;
    }

    public Transaction ToEntity()
        => new (AccountId,
                CategoryId, 
                TransactionTypeId, 
                Description, 
                Amount, 
                Date);
    
}
