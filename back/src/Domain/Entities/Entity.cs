using Domain.Events;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Domain.Entities;

public abstract class Entity<TEntity> : AbstractValidator<TEntity>, IHasDomainEvent where TEntity : Entity<TEntity>
{
    public long Id { get; protected set; }
    public DateTime CreatedDate { get; protected set; }
    public DateTime? LastUpdatedDate { get; protected set; }

    public abstract bool IsValid();

    [NotMapped]
    public ValidationResult ValidationResult { get; protected set; }

    [NotMapped]
    public List<DomainEvent> DomainEvents { get; set; }

    protected Entity()
    {
        ValidationResult = new ValidationResult();
        DomainEvents = [];
    }

    // A definição de CreatedDate e LastUpdatedDate foi centralizada no DbContext
    // (SqlDbContext.SaveChangesAsync). Este método foi removido para evitar
    // chamadas dispersas e garantir que a atribuição de timestamps seja feita
    // de forma consistente e atômica durante a persistência.
}