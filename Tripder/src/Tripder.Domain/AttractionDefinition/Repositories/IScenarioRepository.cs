using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Domain.AttractionDefinition.Repositories;

public interface IScenarioRepository
{
    Task<Scenario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Scenario>> GetByAttractionIdAsync(Guid attractionId, CancellationToken ct = default);
    Task AddAsync(Scenario scenario, CancellationToken ct = default);
    Task UpdateAsync(Scenario scenario, CancellationToken ct = default);
    Task DeleteAsync(Scenario scenario, CancellationToken ct = default);
}

