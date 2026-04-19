using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.DTOs;
using IApplicationRuleDefinitionRepository = Tripder.Application.AttractionDefinition.Repositories.IRuleDefinitionRepository;
using IDomainRuleDefinitionRepository = Tripder.Domain.AttractionDefinition.Repositories.IRuleDefinitionRepository;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class RuleDefinitionRepository : IApplicationRuleDefinitionRepository, IDomainRuleDefinitionRepository
{
    private readonly AppDbContext _db;

    public RuleDefinitionRepository(AppDbContext db)
    {
        _db = db;
    }

    // Application queries
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

    // Domain commands
    async Task<RuleDefinition?> IDomainRuleDefinitionRepository.GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.RuleDefinitions
            .Include(r => r.Days)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    async Task<List<RuleDefinition>> IDomainRuleDefinitionRepository.GetAllAsync(CancellationToken ct)
    {
        return await _db.RuleDefinitions
            .Include(r => r.Days)
            .ToListAsync(ct);
    }

    public async Task AddAsync(RuleDefinition rule, CancellationToken ct = default)
    {
        await _db.RuleDefinitions.AddAsync(rule, ct);
    }

    public Task UpdateAsync(RuleDefinition rule, CancellationToken ct = default)
    {
        _db.RuleDefinitions.Update(rule);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RuleDefinition rule, CancellationToken ct = default)
    {
        _db.RuleDefinitions.Remove(rule);
        return Task.CompletedTask;
    }

    public async Task<string?> GetDayOfWeekNameAsync(Guid dayId, CancellationToken ct = default)
    {
        var entry = await _db.Set<DayOfWeekEntry>()
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == dayId, ct);
        return entry?.Name;
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
