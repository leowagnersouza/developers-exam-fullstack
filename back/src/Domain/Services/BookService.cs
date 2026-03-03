using Domain.Entities;
using Domain.Interfaces;
using FluentValidation.Results;

namespace Domain.Services;

public class BookService : IService<Book>
{
    private readonly IBookRepository _repository;

    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<ValidationResult> InsertAsync(Book entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        // Validação de domínio (tamanho, obrigatoriedade)
        entity.IsValid();
        if (!entity.ValidationResult.IsValid) return entity.ValidationResult;

        // Verifica título duplicado
        if (await _repository.ExistsByTitleAsync(entity.Title))
        {
            var vr = new ValidationResult();
            vr.Errors.Add(new FluentValidation.Results.ValidationFailure("Title", "Título duplicado."));
            return vr;
        }

        await _repository.InsertAsync(entity);

        // Dispara evento de domínio para notificar criação (não envia e-mail diretamente)
        entity.DomainEvents.Add(new Domain.Events.BookCreatedEvent(entity.Title, entity.Author, entity.Description));

        return entity.ValidationResult;
    }

    public async Task<ValidationResult> DeleteAsync(long id)
    {
        var vr = new ValidationResult();
        await _repository.DeleteAsync(id);
        return vr;
    }

    public async Task<ValidationResult> UpdateAsync(Book entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.IsValid();
        if (!entity.ValidationResult.IsValid) return entity.ValidationResult;

        // Título duplicado (exclui o próprio registro)
        if (await _repository.ExistsByTitleAsync(entity.Title, entity.Id))
        {
            var vr = new ValidationResult();
            vr.Errors.Add(new FluentValidation.Results.ValidationFailure("Title", "Título duplicado."));
            return vr;
        }

        await _repository.UpdateAsync(entity);

        return entity.ValidationResult;
    }

    public async Task<Book> GetByIdAsync(long id)
        => await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();

    public async Task<IEnumerable<Book>> GetAllAsync()
        => await _repository.GetAllAsync();
}
