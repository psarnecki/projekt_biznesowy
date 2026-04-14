using Microsoft.EntityFrameworkCore;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Repositories;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class RuleDefinitionRepository : IRuleDefinitionRepository
{
    private readonly AppDbContext _db;

    public RuleDefinitionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(RuleDefinition rule, CancellationToken ct = default)
    {
        await _db.RuleDefinitions.AddAsync(rule, ct);
    }

    public async Task DeleteAsync(RuleDefinition rule, CancellationToken ct = default)
    {
        _db.RuleDefinitions.Remove(rule);
        await Task.CompletedTask;
    }

    public async Task<List<RuleDefinition>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.RuleDefinitions
            .AsNoTracking()
            .Include(r => r.Days)
            .ToListAsync(ct);
    }

    public async Task<RuleDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.RuleDefinitions
            .Include(r => r.Days)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }
}
