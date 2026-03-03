namespace Domain.Queries;

public record GetBooksQuery(int Page = 1, int PageSize = 10);
