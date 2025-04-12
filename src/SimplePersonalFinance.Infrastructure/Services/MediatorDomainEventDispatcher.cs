using MediatR;
using SimplePersonalFinance.Application.Notifications;
using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Infrastructure.Services;

public class MediatorDomainEventDispatcher(IMediator mediator) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
    {
        foreach(var domainEvent in events)
        {
            var notification = Wrap(domainEvent);
            if (notification != null)
                await mediator.Publish(notification);
        }
    }

    private INotification? Wrap(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case BudgetEvaluationRequestedDomainEvent e:
                return new BudgetEvaluationRequestedNotification(e);
            default:
                throw new InvalidOperationException($"No notification found for domain event: {domainEvent.GetType().Name}");
        }
    }
}
