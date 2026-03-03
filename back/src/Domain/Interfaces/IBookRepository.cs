using Domain.Entities;

namespace Domain.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<bool> ExistsByTitleAsync(string title, long? excludeId = null);
}
