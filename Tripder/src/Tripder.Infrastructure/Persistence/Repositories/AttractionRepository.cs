using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.DTOs;
using IApplicationAttractionRepository = Tripder.Application.AttractionDefinition.Repositories.IAttractionRepository;
using IDomainAttractionRepository = Tripder.Domain.AttractionDefinition.Repositories.IAttractionRepository;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class AttractionRepository : IApplicationAttractionRepository, IDomainAttractionRepository
{
    private readonly AppDbContext _db;

    public AttractionRepository(AppDbContext db)
    {
        _db = db;
    }

    // Application queries
    public async Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .Include(a => a.Scenarios)
                .ThenInclude(s => s.Tags)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

        return attraction is null ? null : MapDetail(attraction);
    }

    public async Task<IReadOnlyList<AttractionSummaryDto>> GetAllAsync(string? state = null, CancellationToken ct = default)
    {
        var query = _db.Attractions
            .AsNoTracking()
            .Include(a => a.Category)
            .Include(a => a.Scenarios)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(state) && Enum.TryParse<AttractionState>(state, true, out var parsedState))
        {
            query = query.Where(a => a.State == parsedState);
        }

        return await query
            .Select(a => new AttractionSummaryDto(
                a.Id,
                a.Name,
                a.Category.Name,
                a.Location.LocationName,
                (float)a.Location.Latitude,
                (float)a.Location.Longitude,
                a.State.ToString(),
                a.Scenarios.Count))
            .ToListAsync(ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _db.Attractions.AnyAsync(a => a.Id == id, ct);

    public Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
        => _db.Attractions.AnyAsync(a => a.Name == name && (!excludeId.HasValue || a.Id != excludeId.Value), ct);

    // Domain commands
    async Task<Attraction?> IDomainAttractionRepository.GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Attractions
            .Include(a => a.Category)
            .Include(a => a.Tags)
            .Include(a => a.Rules)
            .Include(a => a.Scenarios)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    async Task<List<Attraction>> IDomainAttractionRepository.GetAllAsync(AttractionState? state, CancellationToken ct)
    {
        var q = _db.Attractions.AsQueryable();
        if (state.HasValue) q = q.Where(a => a.State == state);
        return await q.ToListAsync(ct);
    }

    public async Task AddAsync(Attraction attraction, CancellationToken ct = default)
    {
        await _db.Attractions.AddAsync(attraction, ct);
    }

    public Task UpdateAsync(Attraction attraction, CancellationToken ct = default)
    {
        _db.Attractions.Update(attraction);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Attraction attraction, CancellationToken ct = default)
    {
        _db.Attractions.Remove(attraction);
        return Task.CompletedTask;
    }

    private static AttractionDetailDto MapDetail(Attraction attraction)
        => new(
            attraction.Id,
            attraction.Name,
            attraction.Category.Name,
            attraction.Location.LocationName,
            (float)attraction.Location.Latitude,
            (float)attraction.Location.Longitude,
            attraction.Capacity,
            attraction.State.ToString(),
            attraction.CatalogFrom,
            attraction.CatalogTo,
            attraction.Tags.Select(t => t.Name).ToList(),
            attraction.Scenarios
                .Select(s => new ScenarioSummaryDto(
                    s.Id,
                    s.Name,
                    s.DurationMinutes,
                    s.State.ToString(),
                    s.Tags.Select(t => t.Name).ToList()))
                .ToList());
}
