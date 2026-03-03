using FluentValidation;
using FluentValidation.Results;

namespace Domain.Entities;

/// <summary>
/// Entidade Book representando os livros do sistema.
/// Contém validações de domínio (FluentValidation) e métodos auxiliares.
/// </summary>
public class Book : Entity<Book>
{
    // Propriedades persistidas
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Construtor usado pelo EF
    protected Book() { ConfigureValidation(); }

    // Construtor para uso pela aplicação
    public Book(string title, string author, string? description = null)
    {
        Title = title;
        Author = author;
        Description = description;

        ConfigureValidation();
    }

    // Define regras de validação do domínio usando FluentValidation
    private void ConfigureValidation()
    {
        // Título: obrigatório e entre 10 e 100 caracteres
        RuleFor(b => b.Title)
            .NotEmpty().WithMessage("Título é obrigatório.")
            .Length(10, 100).WithMessage("Título deve ter entre 10 e 100 caracteres.");

        // Autor: obrigatório e entre 10 e 100 caracteres
        RuleFor(b => b.Author)
            .NotEmpty().WithMessage("Autor é obrigatório.")
            .Length(10, 100).WithMessage("Autor deve ter entre 10 e 100 caracteres.");

        // Descrição: opcional, máximo 1024 caracteres
        RuleFor(b => b.Description)
            .MaximumLength(1024).WithMessage("Descrição deve ter no máximo 1024 caracteres.");
    }

    // Executa a validação e atualiza ValidationResult
    public override bool IsValid()
    {
        ValidationResult = Validate(this);
        return ValidationResult.IsValid;
    }

    // Métodos de atualização de propriedades via domínio
    public void UpdateDetails(string title, string author, string? description)
    {
        Title = title;
        Author = author;
        Description = description;
    }
}
