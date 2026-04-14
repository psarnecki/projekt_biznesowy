using MediatR;
using Tripder.Application.AttractionDefinition.DTOs;
using Tripder.Application.AttractionDefinition.Repositories;

namespace Tripder.Application.AttractionDefinition.Queries;

// ───────────────────────────────────────────────
// GET ATTRACTION BY ID
// ───────────────────────────────────────────────

public sealed record GetAttractionByIdQuery(Guid AttractionId) : IRequest<AttractionDetailDto?>;

public sealed class GetAttractionByIdQueryHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<GetAttractionByIdQuery, AttractionDetailDto?>
{
    public Task<AttractionDetailDto?> Handle(GetAttractionByIdQuery query, CancellationToken ct)
        => attractionRepo.GetByIdAsync(query.AttractionId, ct);
}

// ───────────────────────────────────────────────
// GET ALL ATTRACTIONS
// ───────────────────────────────────────────────

public sealed record GetAllAttractionsQuery : IRequest<IReadOnlyList<AttractionSummaryDto>>;

public sealed class GetAllAttractionsQueryHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<GetAllAttractionsQuery, IReadOnlyList<AttractionSummaryDto>>
{
    public Task<IReadOnlyList<AttractionSummaryDto>> Handle(GetAllAttractionsQuery query, CancellationToken ct)
        => attractionRepo.GetAllAsync(ct);
}

// ───────────────────────────────────────────────
// GET SCENARIO BY ID
// ───────────────────────────────────────────────

public sealed record GetScenarioByIdQuery(Guid ScenarioId) : IRequest<ScenarioDetailDto?>;

public sealed class GetScenarioByIdQueryHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<GetScenarioByIdQuery, ScenarioDetailDto?>
{
    public Task<ScenarioDetailDto?> Handle(GetScenarioByIdQuery query, CancellationToken ct)
        => scenarioRepo.GetByIdAsync(query.ScenarioId, ct);
}

// ───────────────────────────────────────────────
// GET SCENARIOS BY ATTRACTION
// ───────────────────────────────────────────────

public sealed record GetScenariosByAttractionQuery(Guid AttractionId) : IRequest<IReadOnlyList<ScenarioSummaryDto>>;

public sealed class GetScenariosByAttractionQueryHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<GetScenariosByAttractionQuery, IReadOnlyList<ScenarioSummaryDto>>
{
    public Task<IReadOnlyList<ScenarioSummaryDto>> Handle(GetScenariosByAttractionQuery query, CancellationToken ct)
        => scenarioRepo.GetByAttractionIdAsync(query.AttractionId, ct);
}

// ───────────────────────────────────────────────
// GET RULE DEFINITION BY ID
// ───────────────────────────────────────────────

public sealed record GetRuleDefinitionByIdQuery(Guid RuleId) : IRequest<RuleDefinitionDto?>;

public sealed class GetRuleDefinitionByIdQueryHandler(
    IRuleDefinitionRepository ruleRepo
) : IRequestHandler<GetRuleDefinitionByIdQuery, RuleDefinitionDto?>
{
    public Task<RuleDefinitionDto?> Handle(GetRuleDefinitionByIdQuery query, CancellationToken ct)
        => ruleRepo.GetByIdAsync(query.RuleId, ct);
}

// ───────────────────────────────────────────────
// GET ALL RULE DEFINITIONS
// ───────────────────────────────────────────────

public sealed record GetAllRuleDefinitionsQuery : IRequest<IReadOnlyList<RuleDefinitionDto>>;

public sealed class GetAllRuleDefinitionsQueryHandler(
    IRuleDefinitionRepository ruleRepo
) : IRequestHandler<GetAllRuleDefinitionsQuery, IReadOnlyList<RuleDefinitionDto>>
{
    public Task<IReadOnlyList<RuleDefinitionDto>> Handle(GetAllRuleDefinitionsQuery query, CancellationToken ct)
        => ruleRepo.GetAllAsync(ct);
}