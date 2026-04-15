using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.DTOs;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;
using Tripder.Domain.AttractionDefinition.ValueObjects;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class AttractionRepository : IAttractionRepository
{
    private readonly AppDbContext _db;

    public AttractionRepository(AppDbContext db)
    {
        _db = db;
    }

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

    public async Task AddAsync(NewAttractionData data, CancellationToken ct = default)
    {
        var attraction = new Attraction(
            data.Id,
            data.Name,
            data.CategoryId,
            new Location(data.Latitude, data.Longitude, data.LocationName),
            data.Capacity,
            data.CatalogFrom,
            data.CatalogTo);

        await _db.Attractions.AddAsync(attraction, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UpdateAttractionData data, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.FirstOrDefaultAsync(a => a.Id == data.Id, ct)
                         ?? throw new KeyNotFoundException($"Attraction {data.Id} not found.");

        _db.Entry(attraction).Property(a => a.Name).CurrentValue = data.Name;
        _db.Entry(attraction).Property(a => a.CategoryId).CurrentValue = data.CategoryId;
        _db.Entry(attraction).Property(a => a.Capacity).CurrentValue = data.Capacity;

        attraction.SetCatalogWindow(data.CatalogFrom, data.CatalogTo);

        var locationEntry = _db.Entry(attraction).Reference(a => a.Location).TargetEntry;
        if (locationEntry is not null)
        {
            locationEntry.Property(nameof(Location.LocationName)).CurrentValue = data.LocationName;
            locationEntry.Property(nameof(Location.Latitude)).CurrentValue = (double)data.Latitude;
            locationEntry.Property(nameof(Location.Longitude)).CurrentValue = (double)data.Longitude;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (attraction is null)
        {
            return;
        }

        _db.Attractions.Remove(attraction);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateStateAsync(Guid id, string newState, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.FirstOrDefaultAsync(a => a.Id == id, ct)
                         ?? throw new KeyNotFoundException($"Attraction {id} not found.");

        if (!Enum.TryParse<AttractionState>(newState, true, out var parsedState))
        {
            throw new ArgumentException($"Invalid attraction state: {newState}", nameof(newState));
        }

        switch (parsedState)
        {
            case AttractionState.Catalog:
                attraction.Publish();
                break;
            case AttractionState.Internal:
                attraction.MakeInternal();
                break;
            case AttractionState.Archived:
                attraction.Archive();
                break;
            case AttractionState.Draft:
                _db.Entry(attraction).Property(a => a.State).CurrentValue = AttractionState.Draft;
                break;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateCatalogWindowAsync(Guid id, DateOnly? catalogFrom, DateOnly? catalogTo, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.FirstOrDefaultAsync(a => a.Id == id, ct)
                         ?? throw new KeyNotFoundException($"Attraction {id} not found.");

        attraction.SetCatalogWindow(catalogFrom, catalogTo);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AssignTagAsync(Guid attractionId, Guid tagId, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == attractionId, ct)
                         ?? throw new KeyNotFoundException($"Attraction {attractionId} not found.");

        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tagId, ct)
                  ?? throw new KeyNotFoundException($"Tag {tagId} not found.");

        attraction.AddTag(tag);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveTagAsync(Guid attractionId, Guid tagId, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == attractionId, ct)
                         ?? throw new KeyNotFoundException($"Attraction {attractionId} not found.");

        attraction.RemoveTag(tagId);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AssignRuleAsync(Guid attractionId, Guid ruleId, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.Include(a => a.Rules).FirstOrDefaultAsync(a => a.Id == attractionId, ct)
                         ?? throw new KeyNotFoundException($"Attraction {attractionId} not found.");

        var rule = await _db.RuleDefinitions.FirstOrDefaultAsync(r => r.Id == ruleId, ct)
                   ?? throw new KeyNotFoundException($"Rule {ruleId} not found.");

        attraction.AddRule(rule);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveRuleAsync(Guid attractionId, Guid ruleId, CancellationToken ct = default)
    {
        var attraction = await _db.Attractions.Include(a => a.Rules).FirstOrDefaultAsync(a => a.Id == attractionId, ct)
                         ?? throw new KeyNotFoundException($"Attraction {attractionId} not found.");

        attraction.RemoveRule(ruleId);
        await _db.SaveChangesAsync(ct);
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
