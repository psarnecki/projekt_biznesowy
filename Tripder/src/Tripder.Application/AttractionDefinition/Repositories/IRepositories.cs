using Tripder.Application.AttractionDefinition.DTOs;

namespace Tripder.Application.AttractionDefinition.Repositories;

public interface IAttractionRepository
{
    Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AttractionSummaryDto>> GetAllAsync(string? state = null, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
}

public interface IScenarioRepository
{
    Task<ScenarioDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ScenarioSummaryDto>> GetByAttractionIdAsync(Guid attractionId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public interface IRuleDefinitionRepository
{
    Task<RuleDefinitionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<RuleDefinitionDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public interface ITagRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<Guid> GetOrCreateByNameAsync(string name, CancellationToken ct = default);
    Task<Guid?> GetIdByNameAsync(string name, CancellationToken ct = default);
}
