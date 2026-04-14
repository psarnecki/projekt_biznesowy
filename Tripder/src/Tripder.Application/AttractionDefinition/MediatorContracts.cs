using MediatR;
using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Application.AttractionDefinition;

public record AttractionSummaryDto(Guid Id, string Name, AttractionState State);

public record AttractionDetailDto(Guid Id, string Name, Guid CategoryId, AttractionState State);

public record ScenarioDetailDto(Guid Id, Guid AttractionId, string Name, ScenarioState State);

public record RuleDefinitionDetailDto(Guid Id, RuleType RuleType, RuleEffect Effect, int Priority);

public record GetAllAttractionsQuery(AttractionState? State = null) : IRequest<IReadOnlyList<AttractionSummaryDto>>;

public record GetAttractionByIdQuery(Guid Id) : IRequest<AttractionDetailDto?>;

public record CreateAttractionCommand(
    string Name,
    Guid CategoryId,
    double Latitude,
    double Longitude,
    string LocationName,
    int? Capacity,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo) : IRequest<Guid>;

public record UpdateAttractionCommand(
    Guid Id,
    string Name,
    Guid CategoryId,
    double Latitude,
    double Longitude,
    string LocationName,
    int? Capacity,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo) : IRequest;

public record PublishAttractionCommand(Guid Id) : IRequest;

public record ArchiveAttractionCommand(Guid Id) : IRequest;

public record DeleteAttractionCommand(Guid Id) : IRequest;

public record AddTagToAttractionCommand(Guid AttractionId, string TagName) : IRequest;

public record RemoveTagFromAttractionCommand(Guid AttractionId, string Tag) : IRequest;

public record AttachRuleToAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public record DetachRuleFromAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public record AddScenarioCommand(
    Guid AttractionId,
    string Name,
    string Description,
    int DurationMinutes) : IRequest<Guid>;

public record UpdateScenarioCommand(
    Guid AttractionId,
    Guid ScenarioId,
    string Name,
    string Description,
    int DurationMinutes) : IRequest;

public record RemoveScenarioCommand(Guid AttractionId, Guid ScenarioId) : IRequest;

public record PublishScenarioCommand(Guid AttractionId, Guid ScenarioId) : IRequest;

public record ArchiveScenarioCommand(Guid AttractionId, Guid ScenarioId) : IRequest;

public record AddTagToScenarioCommand(Guid AttractionId, Guid ScenarioId, string TagName) : IRequest;

public record RemoveTagFromScenarioCommand(Guid AttractionId, Guid ScenarioId, string Tag) : IRequest;

public record AttachRuleToScenarioCommand(Guid AttractionId, Guid ScenarioId, Guid RuleId) : IRequest;

public record DetachRuleFromScenarioCommand(Guid AttractionId, Guid ScenarioId, Guid RuleId) : IRequest;

public record GetScenarioByIdQuery(Guid AttractionId, Guid ScenarioId) : IRequest<ScenarioDetailDto?>;

public record GetAllRuleDefinitionsQuery() : IRequest<IReadOnlyList<RuleDefinitionDetailDto>>;

public record GetRuleDefinitionByIdQuery(Guid Id) : IRequest<RuleDefinitionDetailDto?>;

public record CreateRuleDefinitionCommand(
    RuleType RuleType,
    RuleEffect Effect,
    int Priority,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? RuleParams) : IRequest<Guid>;

public record UpdateRuleDefinitionCommand(
    Guid Id,
    RuleType RuleType,
    RuleEffect Effect,
    int Priority,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? RuleParams) : IRequest;

public record DeleteRuleDefinitionCommand(Guid Id) : IRequest;
