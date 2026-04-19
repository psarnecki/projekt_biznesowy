using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.Repositories;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _db.Categories.AnyAsync(c => c.Id == id, ct);
}

