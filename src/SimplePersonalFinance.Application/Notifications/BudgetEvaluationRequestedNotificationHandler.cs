using MediatR;
using SimplePersonalFinance.Core.Domain.Entities;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Notifications;

public class BudgetEvaluationRequestedNotificationHandler(IUnitOfWork uow) : INotificationHandler<BudgetEvaluationRequestedNotification>
    
{
    public async Task Handle(BudgetEvaluationRequestedNotification notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        await CheckAndNotify(domainEvent.AccountId, domainEvent.Category);

    }

    private async Task CheckAndNotify(Guid accountId,CategoryEnum category)
    {
       
        var account= await uow.Accounts.GetAccountWithTransactionsAsync(accountId) 
                                ?? throw new InvalidOperationException("Account not found");


        var budget = await uow.Budgets.GetByUserAndCategoryAsync(account.UserId, (int)category)
                                        ?? throw new InvalidOperationException("Budget not found");




        decimal totalExpenses = account.Transactions.Where(x=>x.CategoryId==(int)category && 
                                                        x.TransactionTypeId == (int)TransactionTypeEnum.EXPENSE &&
                                                        x.Date.Month==budget.Month &&
                                                        x.Date.Year== budget.Year &&
                                                        x.IsActive)
                                                    .Sum(x=>x.Amount);


        if(budget.LimitAmount < totalExpenses)
        {

            // Notify user about budget limit exceeded
            // This could be an event, a message, or any other notification mechanism
            // For example:
            // await _notificationService.NotifyUser(userId, "Budget limit exceeded", $"Your budget for {category} has been exceeded.");
            Console.WriteLine($"Budget limit exceeded\", $\"Your budget for {category} has been exceeded.");
        }
    }
}
