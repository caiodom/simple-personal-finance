using MediatR;
using SimplePersonalFinance.Application.ViewModels;
using SimplePersonalFinance.Core.Domain.Enums;


namespace SimplePersonalFinance.Application.Commands.EditTransaction;

public class EditAccountTransactionCommand:IRequest<ResultViewModel<Guid>>
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public CategoryEnum CategoryId { get; private set; }
    public TransactionTypeEnum TransactionTypeId { get; private set; }

    public EditAccountTransactionCommand(Guid id, Guid accountId, decimal amount, string description, CategoryEnum categoryId, TransactionTypeEnum transactionTypeId)
    {
        Id = id;
        AccountId = accountId;
        Amount = amount;
        Description = description;
        CategoryId = categoryId;
        TransactionTypeId = transactionTypeId;
    }


}
