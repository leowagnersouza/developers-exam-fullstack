using Domain.Entities;
using FluentValidation.Results;

namespace Domain.Requests;

public record CreateBookCommand(string Title, string Author, string? Description);
