using FluentValidation;
using MediatR;
using Tripder.Application.AttractionDefinition.Repositories;

namespace Tripder.Application.AttractionDefinition.Commands;

// ───────────────────────────────────────────────
// CREATE RULE DEFINITION
// ───────────────────────────────────────────────

public sealed record CreateRuleDefinitionCommand(
    string RuleType,
    string Effect,
    int Priority,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? Params,
    IReadOnlyList<Guid> DayOfWeekIds
) : IRequest<Guid>;

public sealed class CreateRuleDefinitionCommandHandler(
    IRuleDefinitionRepository ruleRepo
) : IRequestHandler<CreateRuleDefinitionCommand, Guid>
{
    public async Task<Guid> Handle(CreateRuleDefinitionCommand cmd, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        await ruleRepo.AddAsync(new NewRuleData(
            id,
            cmd.RuleType,
            cmd.Effect,
            cmd.Priority,
            cmd.TimeFrom,
            cmd.TimeTo,
            cmd.DateFrom,
            cmd.DateTo,
            cmd.Params,
            cmd.DayOfWeekIds
        ), ct);

        return id;
    }
}

public sealed class CreateRuleDefinitionCommandValidator : AbstractValidator<CreateRuleDefinitionCommand>
{
    private static readonly string[] ValidRuleTypes = ["Weekly", "Seasonal", "DateException"];
    private static readonly string[] ValidEffects = ["Allow", "Deny"];

    public CreateRuleDefinitionCommandValidator()
    {
        RuleFor(x => x.RuleType)
            .NotEmpty()
            .Must(t => ValidRuleTypes.Contains(t))
            .WithMessage($"RuleType musi być jedną z wartości: {string.Join(", ", ValidRuleTypes)}.");

        RuleFor(x => x.Effect)
            .NotEmpty()
            .Must(e => ValidEffects.Contains(e))
            .WithMessage($"Effect musi być jedną z wartości: {string.Join(", ", ValidEffects)}.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority musi być wartością nieujemną.");

        RuleFor(x => x.TimeTo)
            .GreaterThan(x => x.TimeFrom).When(x => x.TimeFrom.HasValue && x.TimeTo.HasValue)
            .WithMessage("TimeTo musi być późniejszy niż TimeFrom.");

        RuleFor(x => x.DateTo)
            .GreaterThan(x => x.DateFrom).When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateTo musi być późniejszy niż DateFrom.");

        RuleFor(x => x.DayOfWeekIds)
            .NotNull()
            .Must(ids => ids.All(id => id != Guid.Empty))
            .When(x => x.DayOfWeekIds?.Count > 0)
            .WithMessage("Lista dni tygodnia zawiera nieprawidłowe Id.");
    }
}

// ───────────────────────────────────────────────
// UPDATE RULE DEFINITION
// ───────────────────────────────────────────────

public sealed record UpdateRuleDefinitionCommand(
    Guid RuleId,
    string RuleType,
    string Effect,
    int Priority,
    TimeOnly? TimeFrom,
    TimeOnly? TimeTo,
    DateOnly? DateFrom,
    DateOnly? DateTo,
    string? Params,
    IReadOnlyList<Guid> DayOfWeekIds
) : IRequest;

public sealed class UpdateRuleDefinitionCommandHandler(
    IRuleDefinitionRepository ruleRepo
) : IRequestHandler<UpdateRuleDefinitionCommand>
{
    public async Task Handle(UpdateRuleDefinitionCommand cmd, CancellationToken ct)
        => await ruleRepo.UpdateAsync(new UpdateRuleData(
            cmd.RuleId,
            cmd.RuleType,
            cmd.Effect,
            cmd.Priority,
            cmd.TimeFrom,
            cmd.TimeTo,
            cmd.DateFrom,
            cmd.DateTo,
            cmd.Params,
            cmd.DayOfWeekIds
        ), ct);
}

public sealed class UpdateRuleDefinitionCommandValidator : AbstractValidator<UpdateRuleDefinitionCommand>
{
    private static readonly string[] ValidRuleTypes = ["Weekly", "Seasonal", "DateException"];
    private static readonly string[] ValidEffects = ["Allow", "Deny"];

    public UpdateRuleDefinitionCommandValidator(IRuleDefinitionRepository ruleRepo)
    {
        RuleFor(x => x.RuleId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await ruleRepo.ExistsAsync(id, ct))
            .WithMessage("Reguła o podanym Id nie istnieje.");

        RuleFor(x => x.RuleType)
            .NotEmpty()
            .Must(t => ValidRuleTypes.Contains(t))
            .WithMessage($"RuleType musi być jedną z wartości: {string.Join(", ", ValidRuleTypes)}.");

        RuleFor(x => x.Effect)
            .NotEmpty()
            .Must(e => ValidEffects.Contains(e))
            .WithMessage($"Effect musi być jedną z wartości: {string.Join(", ", ValidEffects)}.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.TimeTo)
            .GreaterThan(x => x.TimeFrom).When(x => x.TimeFrom.HasValue && x.TimeTo.HasValue)
            .WithMessage("TimeTo musi być późniejszy niż TimeFrom.");

        RuleFor(x => x.DateTo)
            .GreaterThan(x => x.DateFrom).When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
            .WithMessage("DateTo musi być późniejszy niż DateFrom.");
    }
}

// ───────────────────────────────────────────────
// DELETE RULE DEFINITION
// ───────────────────────────────────────────────

public sealed record DeleteRuleDefinitionCommand(Guid RuleId) : IRequest;

public sealed class DeleteRuleDefinitionCommandHandler(
    IRuleDefinitionRepository ruleRepo
) : IRequestHandler<DeleteRuleDefinitionCommand>
{
    public async Task Handle(DeleteRuleDefinitionCommand cmd, CancellationToken ct)
        => await ruleRepo.DeleteAsync(cmd.RuleId, ct);
}

public sealed class DeleteRuleDefinitionCommandValidator : AbstractValidator<DeleteRuleDefinitionCommand>
{
    public DeleteRuleDefinitionCommandValidator(IRuleDefinitionRepository ruleRepo)
    {
        RuleFor(x => x.RuleId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await ruleRepo.ExistsAsync(id, ct))
            .WithMessage("Reguła o podanym Id nie istnieje.");
    }
}