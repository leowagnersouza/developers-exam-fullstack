using Domain.Events;
using Domain.Interfaces;
using MediatR;

namespace Infrastructure.Events;

public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
{
    public DomainEventNotification(TDomainEvent domainEvent)
        => DomainEvent = domainEvent;

    public TDomainEvent DomainEvent { get; }
}

public class DomainEventHandler : IDomainEventHandler
{
    private readonly IPublisher _mediator;

    public DomainEventHandler(IPublisher mediator)
        => _mediator = mediator;

    public async Task Publish(DomainEvent domainEvent)
        => await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));

    private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        var notificationInstance = Activator.CreateInstance(notificationType, domainEvent);

        if (notificationInstance is not INotification notification)
        {
            throw new InvalidOperationException($"Could not create notification for domain event type {domainEvent.GetType().FullName}.");
        }

        return notification;
    }
}