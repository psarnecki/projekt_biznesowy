using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.DTOs;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class RuleDefinitionRepository : IRuleDefinitionRepository
{
    private readonly AppDbContext _db;

    public RuleDefinitionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<RuleDefinitionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var rule = await _db.RuleDefinitions
            .AsNoTracking()
            .Include(r => r.Days)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        return rule is null ? null : Map(rule);
    }

    public async Task<IReadOnlyList<RuleDefinitionDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.RuleDefinitions
            .AsNoTracking()
            .Include(r => r.Days)
            .Select(r => Map(r))
            .ToListAsync(ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _db.RuleDefinitions.AnyAsync(r => r.Id == id, ct);

    public async Task AddAsync(NewRuleData data, CancellationToken ct = default)
    {
        var rule = new RuleDefinition(
            data.Id,
            ParseRuleType(data.RuleType),
            ParseRuleEffect(data.Effect),
            data.Priority,
            data.TimeFrom,
            data.TimeTo,
            data.DateFrom,
            data.DateTo,
            data.Params);

        await _db.RuleDefinitions.AddAsync(rule, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UpdateRuleData data, CancellationToken ct = default)
    {
        var rule = await _db.RuleDefinitions
            .Include(r => r.Days)
            .FirstOrDefaultAsync(r => r.Id == data.Id, ct)
            ?? throw new KeyNotFoundException($"Rule {data.Id} not found.");

        _db.Entry(rule).Property(r => r.RuleType).CurrentValue = ParseRuleType(data.RuleType);
        _db.Entry(rule).Property(r => r.Effect).CurrentValue = ParseRuleEffect(data.Effect);
        _db.Entry(rule).Property(r => r.Priority).CurrentValue = data.Priority;
        _db.Entry(rule).Property(r => r.TimeFrom).CurrentValue = data.TimeFrom;
        _db.Entry(rule).Property(r => r.TimeTo).CurrentValue = data.TimeTo;
        _db.Entry(rule).Property(r => r.DateFrom).CurrentValue = data.DateFrom;
        _db.Entry(rule).Property(r => r.DateTo).CurrentValue = data.DateTo;
        _db.Entry(rule).Property(r => r.Params).CurrentValue = data.Params;

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var rule = await _db.RuleDefinitions.FirstOrDefaultAsync(r => r.Id == id, ct);
        if (rule is null)
        {
            return;
        }

        _db.RuleDefinitions.Remove(rule);
        await _db.SaveChangesAsync(ct);
    }

    private static RuleType ParseRuleType(string value)
    {
        if (!Enum.TryParse<RuleType>(value, true, out var parsed))
        {
            throw new ArgumentException($"Invalid rule type: {value}", nameof(value));
        }

        return parsed;
    }

    private static RuleEffect ParseRuleEffect(string value)
    {
        if (!Enum.TryParse<RuleEffect>(value, true, out var parsed))
        {
            throw new ArgumentException($"Invalid rule effect: {value}", nameof(value));
        }

        return parsed;
    }

    private static RuleDefinitionDto Map(RuleDefinition rule)
        => new(
            rule.Id,
            rule.RuleType.ToString(),
            rule.Effect.ToString(),
            rule.Priority,
            rule.TimeFrom,
            rule.TimeTo,
            rule.DateFrom,
            rule.DateTo,
            rule.Params,
            rule.Days.Select(d => d.Name).ToList());
}
