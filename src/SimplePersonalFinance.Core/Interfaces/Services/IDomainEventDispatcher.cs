using SimplePersonalFinance.Core.Domain.Entities.Base;

namespace SimplePersonalFinance.Core.Interfaces.Services;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> events);
}
