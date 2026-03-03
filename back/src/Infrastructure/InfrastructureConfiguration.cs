using System.Reflection;
using Domain.Interfaces;
using Domain.Services;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Infrastructure.Services;
using Infrastructure.Events;
using Domain.Events;

namespace Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Registra o DbContext do EF Core. O método AddDbContext já registra o contexto
        // com lifetime Scoped, portanto não é necessário chamar AddScoped<SqlDbContext>().
        // Para debugging local, usa-se uma connection string hardcoded que se mostrou
        // funcional contra o SQL Server em Docker/localhost. Remova ou revert a alteração
        // para usar configuration.GetConnectionString(...) antes de commitar em produção.
        var hardcodedConnection = "Server=127.0.0.1,1433;Database=INSPAND_Exam;User Id=sa;Password=5Gi3re61sXehuSGxMqHX;Encrypt=False;TrustServerCertificate=True;";
        services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(hardcodedConnection));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });
        
        services.AddScoped<IDomainEventHandler, DomainEventHandler>();

        // Registra repositório e serviço de domínio para Book
        services.AddScoped<IBookRepository, BookRepository>();
        // Registra BookService como implementação de IService<Book>
        services.AddScoped<IService<Domain.Entities.Book>, BookService>();

        // Registra serviço de e-mail (simulado) e handler para evento de criação de livro
        services.AddScoped<Domain.Interfaces.IEmailService, EmailService>();
        services.AddScoped<INotificationHandler<DomainEventNotification<BookCreatedEvent>>, BookCreatedEventHandler>();

        return services;
    }
}