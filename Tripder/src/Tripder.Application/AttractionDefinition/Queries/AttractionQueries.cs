using MediatR;
using Tripder.Application.AttractionDefinition.DTOs;
using Tripder.Application.AttractionDefinition.Repositories;

namespace Tripder.Application.AttractionDefinition.Queries;

// Get attraction by id
public sealed record GetAttractionByIdQuery(Guid AttractionId) : IRequest<AttractionDetailDto?>;

public sealed class GetAttractionByIdQueryHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<GetAttractionByIdQuery, AttractionDetailDto?>
{
    public Task<AttractionDetailDto?> Handle(GetAttractionByIdQuery query, CancellationToken ct)
        => attractionRepo.GetByIdAsync(query.AttractionId, ct);
}

// Get all attractions
public sealed record GetAllAttractionsQuery(string? State = null) : IRequest<IReadOnlyList<AttractionSummaryDto>>;

public sealed class GetAllAttractionsQueryHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<GetAllAttractionsQuery, IReadOnlyList<AttractionSummaryDto>>
{
    public Task<IReadOnlyList<AttractionSummaryDto>> Handle(GetAllAttractionsQuery query, CancellationToken ct)
        => attractionRepo.GetAllAsync(query.State, ct);
}

// Get scenario by id
public sealed record GetScenarioByIdQuery(Guid AttractionId, Guid ScenarioId) : IRequest<ScenarioDetailDto?>;

public sealed class GetScenarioByIdQueryHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<GetScenarioByIdQuery, ScenarioDetailDto?>
{
    public Task<ScenarioDetailDto?> Handle(GetScenarioByIdQuery query, CancellationToken ct)
        => scenarioRepo.GetByIdAsync(query.ScenarioId, ct);
}

// Get scenarios by attraction
public sealed record GetScenariosByAttractionQuery(Guid AttractionId) : IRequest<IReadOnlyList<ScenarioSummaryDto>>;

public sealed class GetScenariosByAttractionQueryHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<GetScenariosByAttractionQuery, IReadOnlyList<ScenarioSummaryDto>>
{
    public Task<IReadOnlyList<ScenarioSummaryDto>> Handle(GetScenariosByAttractionQuery query, CancellationToken ct)
        => scenarioRepo.GetByAttractionIdAsync(query.AttractionId, ct);
}

// Get rule definition by id
public sealed record GetRuleDefinitionByIdQuery(Guid RuleId) : IRequest<RuleDefinitionDto?>;

public sealed class GetRuleDefinitionByIdQueryHandler(
    IRuleDefinitionRepository ruleRepo
) : IRequestHandler<GetRuleDefinitionByIdQuery, RuleDefinitionDto?>
{
    public Task<RuleDefinitionDto?> Handle(GetRuleDefinitionByIdQuery query, CancellationToken ct)
        => ruleRepo.GetByIdAsync(query.RuleId, ct);
}

// Get all rule definitions
public sealed record GetAllRuleDefinitionsQuery : IRequest<IReadOnlyList<RuleDefinitionDto>>;

public sealed class GetAllRuleDefinitionsQueryHandler(
    IRuleDefinitionRepository ruleRepo
) : IRequestHandler<GetAllRuleDefinitionsQuery, IReadOnlyList<RuleDefinitionDto>>
{
    public Task<IReadOnlyList<RuleDefinitionDto>> Handle(GetAllRuleDefinitionsQuery query, CancellationToken ct)
        => ruleRepo.GetAllAsync(ct);
}