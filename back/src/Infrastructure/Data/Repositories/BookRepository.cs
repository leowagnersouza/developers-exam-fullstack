using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(SqlDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsByTitleAsync(string title, long? excludeId = null)
    {
        if (excludeId.HasValue)
            return await DbSet.AnyAsync(b => b.Title == title && b.Id != excludeId.Value);

        return await DbSet.AnyAsync(b => b.Title == title);
    }
}
