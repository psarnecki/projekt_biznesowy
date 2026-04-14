using Microsoft.EntityFrameworkCore;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;
using Tripder.Domain.AttractionDefinition.Repositories;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class AttractionRepository : IAttractionRepository
{
    private readonly AppDbContext _db;

    public AttractionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Attraction attraction, CancellationToken ct = default)
    {
        // Add = wrzuć do kontekstu EF, ale SaveChanges robi UnitOfWork (żeby nie dublować transakcji jak w labach)
        await _db.Attractions.AddAsync(attraction, ct);
    }

    public async Task DeleteAsync(Attraction attraction, CancellationToken ct = default)
    {
        _db.Attractions.Remove(attraction);
        await Task.CompletedTask;
    }

    public async Task<List<Attraction>> GetAllAsync(AttractionState? state = null, CancellationToken ct = default)
    {
        // filtr opcjonalny — jak state == null to bierzemy wszystko, proste jak drut
        var q = _db.Attractions.AsNoTracking().AsQueryable();
        if (state.HasValue) q = q.Where(a => a.State == state);
        return await q.ToListAsync(ct);
    }

    public async Task<Attraction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // split query bo inaczej EF robi wielkie kartezjańskie joiny i potem się dziwisz że RAM znika xD
        return await _db.Attractions
            .AsSplitQuery()
            .Include(a => a.Category)
            .Include(a => a.Scenarios)
            .ThenInclude(s => s.Images)
            .Include(a => a.Scenarios)
            .ThenInclude(s => s.Tags)
            .Include(a => a.Scenarios)
            .ThenInclude(s => s.Rules)
            .Include(a => a.Tags)
            .Include(a => a.Rules)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    public Task UpdateAsync(Attraction attraction, CancellationToken ct = default)
    {
        // Update oznacza "cały graf jest zmieniony" — w prostych projektach tak się robi; jak coś wybucha to wtedy się robi tracking ręcznie
        _db.Attractions.Update(attraction);
        return Task.CompletedTask;
    }
}
