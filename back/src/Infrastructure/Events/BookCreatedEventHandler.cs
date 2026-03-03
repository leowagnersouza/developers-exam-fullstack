using Domain.Events;
using Domain.Interfaces;
using MediatR;

namespace Infrastructure.Events;

public class BookCreatedEventHandler : INotificationHandler<DomainEventNotification<BookCreatedEvent>>
{
    private readonly Domain.Interfaces.IEmailService _emailService;

    public BookCreatedEventHandler(Domain.Interfaces.IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task Handle(DomainEventNotification<BookCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var evt = notification.DomainEvent;

        // Apenas dispara o envio via IEmailService (implementação de envio está em Infrastructure.Services.EmailService)
        _emailService.SendEmail("developers@inspand.com.br", $"Novo livro criado: {evt.Title} por {evt.Author}");

        return Task.CompletedTask;
    }
}
