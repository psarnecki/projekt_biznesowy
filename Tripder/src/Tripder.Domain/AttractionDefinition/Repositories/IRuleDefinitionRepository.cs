using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Domain.AttractionDefinition.Repositories;

public interface IRuleDefinitionRepository
{
    Task<RuleDefinition?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<RuleDefinition>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(RuleDefinition rule, CancellationToken ct = default);
    Task DeleteAsync(RuleDefinition rule, CancellationToken ct = default);
}
