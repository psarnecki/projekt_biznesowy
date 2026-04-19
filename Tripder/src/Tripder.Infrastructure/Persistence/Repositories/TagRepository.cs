using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class TagRepository : ITagRepository
{
    private readonly AppDbContext _db;

    public TagRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _db.Tags.AnyAsync(t => t.Id == id, ct);

    public async Task<Guid> GetOrCreateByNameAsync(string name, CancellationToken ct = default)
    {
        var normalized = Normalize(name);
        var lowered = normalized.ToLower();

        var existing = await _db.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == lowered, ct);

        if (existing is not null)
        {
            return existing.Id;
        }

        var newTag = new Tag(Guid.NewGuid(), normalized);
        await _db.Tags.AddAsync(newTag, ct);
        await _db.SaveChangesAsync(ct);

        return newTag.Id;
    }

    public async Task<Guid?> GetIdByNameAsync(string name, CancellationToken ct = default)
    {
        var normalized = Normalize(name);
        var lowered = normalized.ToLower();

        return await _db.Tags
            .Where(t => t.Name.ToLower() == lowered)
            .Select(t => (Guid?)t.Id)
            .FirstOrDefaultAsync(ct);
    }

    private static string Normalize(string name)
    {
        var normalized = name.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Tag name cannot be empty.", nameof(name));
        }

        return normalized;
    }
}

