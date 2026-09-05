using MediatR;
using Microsoft.Extensions.Logging;
using SimplePersonalFinance.Application.Notifications;
using SimplePersonalFinance.Core.Domain.Entities.Base;
using SimplePersonalFinance.Core.Domain.Events;
using SimplePersonalFinance.Core.Interfaces.Services;

namespace SimplePersonalFinance.Infrastructure.Services;

public class MediatorDomainEventDispatcher(
    IMediator mediator,
    ILogger<MediatorDomainEventDispatcher> logger) : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken cancellationToken)
    {
        foreach (var domainEvent in events)
        {
            var notification = Wrap(domainEvent);

            logger.LogInformation(
                "Dispatching domain event {EventName} from {EntityType} with ID {EntityId}",
                domainEvent.GetType().Name,
                domainEvent.EntityType,
                domainEvent.EntityId);

            await mediator.Publish(notification, cancellationToken);
        }
    }

    private static INotification Wrap(IDomainEvent domainEvent)
        => domainEvent switch
        {
            BudgetEvaluationRequestedDomainEvent e => new BudgetEvaluationRequestedNotification(e),
            _ => throw new InvalidOperationException($"No notification found for domain event: {domainEvent.GetType().Name}")
        };
}
