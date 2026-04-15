using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.DTOs;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class ScenarioRepository : IScenarioRepository
{
    private readonly AppDbContext _db;

    public ScenarioRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ScenarioDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios
            .AsNoTracking()
            .Include(s => s.Tags)
            .Include(s => s.Images)
            .Include(s => s.Rules)
                .ThenInclude(r => r.Days)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        return scenario is null ? null : MapDetail(scenario);
    }

    public async Task<IReadOnlyList<ScenarioSummaryDto>> GetByAttractionIdAsync(Guid attractionId, CancellationToken ct = default)
    {
        return await _db.Scenarios
            .AsNoTracking()
            .Where(s => s.AttractionId == attractionId)
            .Include(s => s.Tags)
            .Select(s => new ScenarioSummaryDto(
                s.Id,
                s.Name,
                s.DurationMinutes,
                s.State.ToString(),
                s.Tags.Select(t => t.Name).ToList()))
            .ToListAsync(ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _db.Scenarios.AnyAsync(s => s.Id == id, ct);

    public async Task AddAsync(NewScenarioData data, CancellationToken ct = default)
    {
        var scenario = new Scenario(
            data.Id,
            data.AttractionId,
            data.Name,
            data.Description,
            data.DurationMinutes);

        await _db.Scenarios.AddAsync(scenario, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UpdateScenarioData data, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios.FirstOrDefaultAsync(s => s.Id == data.Id, ct)
                       ?? throw new KeyNotFoundException($"Scenario {data.Id} not found.");

        scenario.Update(data.Name, data.Description, data.DurationMinutes);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (scenario is null)
        {
            return;
        }

        _db.Scenarios.Remove(scenario);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateStateAsync(Guid id, string newState, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios.FirstOrDefaultAsync(s => s.Id == id, ct)
                       ?? throw new KeyNotFoundException($"Scenario {id} not found.");

        if (!Enum.TryParse<ScenarioState>(newState, true, out var parsedState))
        {
            throw new ArgumentException($"Invalid scenario state: {newState}", nameof(newState));
        }

        switch (parsedState)
        {
            case ScenarioState.Catalog:
                scenario.Publish();
                break;
            case ScenarioState.Archived:
                scenario.Archive();
                break;
            case ScenarioState.Internal:
                _db.Entry(scenario).Property(s => s.State).CurrentValue = ScenarioState.Internal;
                break;
            case ScenarioState.Draft:
                _db.Entry(scenario).Property(s => s.State).CurrentValue = ScenarioState.Draft;
                break;
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task AssignTagAsync(Guid scenarioId, Guid tagId, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios.Include(s => s.Tags).FirstOrDefaultAsync(s => s.Id == scenarioId, ct)
                       ?? throw new KeyNotFoundException($"Scenario {scenarioId} not found.");

        var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tagId, ct)
                  ?? throw new KeyNotFoundException($"Tag {tagId} not found.");

        scenario.AddTag(tag);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveTagAsync(Guid scenarioId, Guid tagId, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios.Include(s => s.Tags).FirstOrDefaultAsync(s => s.Id == scenarioId, ct)
                       ?? throw new KeyNotFoundException($"Scenario {scenarioId} not found.");

        scenario.RemoveTag(tagId);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AssignRuleAsync(Guid scenarioId, Guid ruleId, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios.Include(s => s.Rules).FirstOrDefaultAsync(s => s.Id == scenarioId, ct)
                       ?? throw new KeyNotFoundException($"Scenario {scenarioId} not found.");

        var rule = await _db.RuleDefinitions.FirstOrDefaultAsync(r => r.Id == ruleId, ct)
                   ?? throw new KeyNotFoundException($"Rule {ruleId} not found.");

        scenario.AddRule(rule);
        await _db.SaveChangesAsync(ct);
    }

    public async Task RemoveRuleAsync(Guid scenarioId, Guid ruleId, CancellationToken ct = default)
    {
        var scenario = await _db.Scenarios.Include(s => s.Rules).FirstOrDefaultAsync(s => s.Id == scenarioId, ct)
                       ?? throw new KeyNotFoundException($"Scenario {scenarioId} not found.");

        scenario.RemoveRule(ruleId);
        await _db.SaveChangesAsync(ct);
    }

    private static ScenarioDetailDto MapDetail(Scenario scenario)
        => new(
            scenario.Id,
            scenario.AttractionId,
            scenario.Name,
            scenario.Description,
            scenario.DurationMinutes,
            scenario.State.ToString(),
            scenario.Tags.Select(t => t.Name).ToList(),
            scenario.Images
                .OrderBy(i => i.OrderIndex)
                .Select(i => new ImageDto(i.Id, i.Url, i.OrderIndex))
                .ToList(),
            scenario.Rules
                .Select(r => new RuleDefinitionDto(
                    r.Id,
                    r.RuleType.ToString(),
                    r.Effect.ToString(),
                    r.Priority,
                    r.TimeFrom,
                    r.TimeTo,
                    r.DateFrom,
                    r.DateTo,
                    r.Params,
                    r.Days.Select(d => d.Name).ToList()))
                .ToList());
}

