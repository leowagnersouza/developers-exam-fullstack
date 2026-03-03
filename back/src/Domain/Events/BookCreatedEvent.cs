namespace Domain.Events;

public class BookCreatedEvent : DomainEvent
{
    public string Title { get; }
    public string Author { get; }
    public string? Description { get; }

    public BookCreatedEvent(string title, string author, string? description = null)
    {
        Title = title;
        Author = author;
        Description = description;
    }
}
