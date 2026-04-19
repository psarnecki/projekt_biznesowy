using Microsoft.EntityFrameworkCore;
using Tripder.Application.AttractionDefinition.DTOs;
using IApplicationScenarioRepository = Tripder.Application.AttractionDefinition.Repositories.IScenarioRepository;
using IDomainScenarioRepository = Tripder.Domain.AttractionDefinition.Repositories.IScenarioRepository;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Repositories;

public class ScenarioRepository : IApplicationScenarioRepository, IDomainScenarioRepository
{
    private readonly AppDbContext _db;

    public ScenarioRepository(AppDbContext db)
    {
        _db = db;
    }

    // Application queries
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

    // Domain commands
    async Task<Scenario?> IDomainScenarioRepository.GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Scenarios
            .Include(s => s.Tags)
            .Include(s => s.Images)
            .Include(s => s.Rules)
            .FirstOrDefaultAsync(s => s.Id == id, ct);
    }

    async Task<List<Scenario>> IDomainScenarioRepository.GetByAttractionIdAsync(Guid attractionId, CancellationToken ct)
    {
        return await _db.Scenarios
            .Include(s => s.Tags)
            .Include(s => s.Images)
            .Include(s => s.Rules)
            .Where(s => s.AttractionId == attractionId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Scenario scenario, CancellationToken ct = default)
    {
        await _db.Scenarios.AddAsync(scenario, ct);
    }

    public Task UpdateAsync(Scenario scenario, CancellationToken ct = default)
    {
        _db.Scenarios.Update(scenario);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Scenario scenario, CancellationToken ct = default)
    {
        _db.Scenarios.Remove(scenario);
        return Task.CompletedTask;
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
