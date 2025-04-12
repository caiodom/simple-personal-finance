using MediatR;
using SimplePersonalFinance.Core.Domain.Enums;
using SimplePersonalFinance.Core.Interfaces.Data;

namespace SimplePersonalFinance.Application.Notifications;

public class BudgetEvaluationRequestedNotificationHandler(IUnitOfWork uow) : INotificationHandler<BudgetEvaluationRequestedNotification>
{
    public async Task Handle(BudgetEvaluationRequestedNotification notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        await CheckAndNotify(domainEvent.AccountId, domainEvent.UserId, domainEvent.Category);
    }

    private async Task CheckAndNotify(Guid accountId, Guid userId, CategoryEnum category)
    {
        var budget = await uow.Budgets.GetByUserAndCategoryAsync(userId, (int)category)
                                        ?? throw new InvalidOperationException("Budget not found");

        var transactions = await uow.Transactions.GetCategoryExpensesByAccountAndPeriod(
            accountId,
            category,
            new DateTime(budget.Year, budget.Month, 1));

        decimal totalExpenses = transactions.Sum(x => x.Amount);

        if (budget.LimitAmount < totalExpenses)
        {
            // Notify user about budget limit exceeded
            Console.WriteLine($"Budget limit exceeded. Your budget for {category} has been exceeded.");
        }
    }
}
