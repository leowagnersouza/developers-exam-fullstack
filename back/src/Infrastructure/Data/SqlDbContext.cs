using System.Reflection;
using Domain.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data;

public class SqlDbContext : DbContext
{
    private readonly IDomainEventHandler _domainEventService;

    public SqlDbContext(DbContextOptions<SqlDbContext> options, IDomainEventHandler domainEventService) : base(options)
        => _domainEventService = domainEventService;

    // DbSet para a entidade Book
    public DbSet<Book> Books { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Centraliza a definição de timestamps (CreatedDate / LastUpdatedDate)
        // diretamente no DbContext para garantir comportamento consistente.
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity != null && (e.State == EntityState.Added || e.State == EntityState.Modified));

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            try
            {
                var entity = entry.Entity;

                // Se for adicionado, define CreatedDate
                if (entry.State == EntityState.Added)
                {
                    var createdProp = entity.GetType().GetProperty("CreatedDate");
                    if (createdProp != null && createdProp.CanWrite)
                        createdProp.SetValue(entity, now);
                }

                // Se for modificado, define LastUpdatedDate
                if (entry.State == EntityState.Modified)
                {
                    var updatedProp = entity.GetType().GetProperty("LastUpdatedDate");
                    if (updatedProp != null && updatedProp.CanWrite)
                        updatedProp.SetValue(entity, now);
                }
            }
            catch
            {
                // Em caso de falha ao definir timestamps, não interrompe a operação de persistência
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(dateTimeConverter);
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    private async Task DispatchEvents()
    {
        while (true)
        {
            try
            {
                var domainEventEntity = ChangeTracker.Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .Where(domainEvent => !domainEvent.IsPublished)
                    .FirstOrDefault();

                if (domainEventEntity is null) break;

                domainEventEntity.IsPublished = true;

                await _domainEventService.Publish(domainEventEntity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}