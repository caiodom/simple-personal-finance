using MediatR;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data.Repositories;

namespace SimplePersonalFinance.Application.Notifications;

public class BudgetEvaluationRequestedNotificationHandler(
    IBudgetRepository budgets,
    ITransactionRepository transactions) : INotificationHandler<BudgetEvaluationRequestedNotification>
{
    public async Task Handle(BudgetEvaluationRequestedNotification notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        await CheckAndNotify(domainEvent.AccountId, domainEvent.UserId, domainEvent.Category);
    }

    private async Task CheckAndNotify(Guid accountId, Guid userId, CategoryEnum category)
    {
        var budget = await budgets.GetByUserAndCategoryAsync(userId, (int)category);

        if (budget == null)
            return;

        var categoryExpenses = await transactions.GetCategoryExpensesByAccountAndPeriod(
            accountId,
            category,
            new DateTime(budget.Year, budget.Month, 1));

        var totalExpenses = categoryExpenses.Sum(transaction => transaction.Amount);

        if (budget.LimitAmount < totalExpenses)
        {
            Console.WriteLine($"Budget limit exceeded. Your budget for {category} has been exceeded.");
        }
    }
}
