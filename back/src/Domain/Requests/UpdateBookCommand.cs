namespace Domain.Requests;

public record UpdateBookCommand(long Id, string Title, string Author, string? Description);
