namespace AttractionDefinition.Dtos;

public sealed record AttractionDetailDto(
    Guid Id,
    string Name,
    string Category,
    string LocationName,
    float Latitude,
    float Longitude,
    int? Capacity,
    string State,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo,
    IReadOnlyList<string> Tags,
    IReadOnlyList<ScenarioSummaryDto> Scenarios
);

public sealed record AttractionSummaryDto(
    Guid Id,
    string Name,
    string Category,
    string LocationName,
    float Latitude,
    float Longitude,
    string State,
    int ScenarioCount
);

public sealed record ScenarioDetailDto(
    Guid Id,
    Guid AttractionId,
    string Name,
    string Description,
    int DurationMinutes,
    string State,
    IReadOnlyList<string> Tags,
    IReadOnlyList<ImageDto> Images,
    IReadOnlyList<RuleDefinitionDto> Rules
);

public sealed record ScenarioSummaryDto(
    Guid Id,
    string Name,
    int DurationMinutes,
    string State,
    IReadOnlyList<string> Tags
);

public sealed record ImageDto(
    Guid Id,
    string Url,
    int OrderIndex
);

public sealed record RuleDefinitionDto(
    Guid Id,
    string RuleType,
    string Effect,
    int Priority,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? Params,
    IReadOnlyList<string> DaysOfWeek
);