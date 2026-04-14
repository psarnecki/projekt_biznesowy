using AttractionDefinition.Dtos;

namespace AttractionDefinition.Interfaces;

public interface IAttractionRepository
{
    Task<AttractionDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AttractionSummaryDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    Task AddAsync(NewAttractionData data, CancellationToken ct = default);
    Task UpdateStateAsync(Guid id, string newState, CancellationToken ct = default);
    Task UpdateCatalogWindowAsync(Guid id, DateOnly? catalogFrom, DateOnly? catalogTo, CancellationToken ct = default);
    Task AssignTagAsync(Guid attractionId, Guid tagId, CancellationToken ct = default);
    Task RemoveTagAsync(Guid attractionId, Guid tagId, CancellationToken ct = default);
    Task AssignRuleAsync(Guid attractionId, Guid ruleId, CancellationToken ct = default);
    Task RemoveRuleAsync(Guid attractionId, Guid ruleId, CancellationToken ct = default);
}

public interface IScenarioRepository
{
    Task<ScenarioDetailDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ScenarioSummaryDto>> GetByAttractionIdAsync(Guid attractionId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(NewScenarioData data, CancellationToken ct = default);
    Task UpdateStateAsync(Guid id, string newState, CancellationToken ct = default);
    Task AssignTagAsync(Guid scenarioId, Guid tagId, CancellationToken ct = default);
    Task RemoveTagAsync(Guid scenarioId, Guid tagId, CancellationToken ct = default);
    Task AssignRuleAsync(Guid scenarioId, Guid ruleId, CancellationToken ct = default);
    Task RemoveRuleAsync(Guid scenarioId, Guid ruleId, CancellationToken ct = default);
}

public interface IRuleDefinitionRepository
{
    Task<RuleDefinitionDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<RuleDefinitionDto>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(NewRuleData data, CancellationToken ct = default);
    Task UpdateAsync(UpdateRuleData data, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public interface ITagRepository
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

// Wewnętrzne rekordy danych dla operacji zapisu (nie wychodzą poza Application)
public sealed record NewAttractionData(
    Guid Id,
    string Name,
    Guid CategoryId,
    string LocationName,
    float Latitude,
    float Longitude,
    int? Capacity,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo
);

public sealed record NewScenarioData(
    Guid Id,
    Guid AttractionId,
    string Name,
    string Description,
    int DurationMinutes
);

public sealed record NewRuleData(
    Guid Id,
    string RuleType,
    string Effect,
    int Priority,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? Params,
    IReadOnlyList<Guid> DayOfWeekIds
);

public sealed record UpdateRuleData(
    Guid Id,
    string RuleType,
    string Effect,
    int Priority,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? Params,
    IReadOnlyList<Guid> DayOfWeekIds
);