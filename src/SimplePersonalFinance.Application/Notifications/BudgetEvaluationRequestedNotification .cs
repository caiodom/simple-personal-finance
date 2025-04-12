using MediatR;
using SimplePersonalFinance.Core.Domain.Events;

namespace SimplePersonalFinance.Application.Notifications;

public class BudgetEvaluationRequestedNotification:INotification
{
    public BudgetEvaluationRequestedDomainEvent DomainEvent { get;}

    public BudgetEvaluationRequestedNotification(BudgetEvaluationRequestedDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }


}
