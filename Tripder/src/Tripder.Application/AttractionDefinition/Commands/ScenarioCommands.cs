using AttractionDefinition.Interfaces;
using FluentValidation;
using MediatR;

namespace AttractionDefinition.Commands;

// ───────────────────────────────────────────────
// CREATE SCENARIO
// ───────────────────────────────────────────────

public sealed record CreateScenarioCommand(
    Guid AttractionId,
    string Name,
    string Description,
    int DurationMinutes
) : IRequest<Guid>;

public sealed class CreateScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<CreateScenarioCommand, Guid>
{
    public async Task<Guid> Handle(CreateScenarioCommand cmd, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        await scenarioRepo.AddAsync(new NewScenarioData(
            id,
            cmd.AttractionId,
            cmd.Name,
            cmd.Description,
            cmd.DurationMinutes
        ), ct);

        return id;
    }
}

public sealed class CreateScenarioCommandValidator : AbstractValidator<CreateScenarioCommand>
{
    public CreateScenarioCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa scenariusza jest wymagana.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Opis scenariusza jest wymagany.")
            .MaximumLength(2000);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Czas trwania musi być wartością dodatnią.")
            .LessThanOrEqualTo(1440).WithMessage("Czas trwania nie może przekraczać 1440 minut (24h).");
    }
}

// ───────────────────────────────────────────────
// PUBLISH SCENARIO (Draft → Catalog)
// ───────────────────────────────────────────────

public sealed record PublishScenarioCommand(Guid ScenarioId) : IRequest;

public sealed class PublishScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<PublishScenarioCommand>
{
    public async Task Handle(PublishScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.UpdateStateAsync(cmd.ScenarioId, "Catalog", ct);
}

public sealed class PublishScenarioCommandValidator : AbstractValidator<PublishScenarioCommand>
{
    public PublishScenarioCommandValidator(IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// ARCHIVE SCENARIO
// ───────────────────────────────────────────────

public sealed record ArchiveScenarioCommand(Guid ScenarioId) : IRequest;

public sealed class ArchiveScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<ArchiveScenarioCommand>
{
    public async Task Handle(ArchiveScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.UpdateStateAsync(cmd.ScenarioId, "Archived", ct);
}

public sealed class ArchiveScenarioCommandValidator : AbstractValidator<ArchiveScenarioCommand>
{
    public ArchiveScenarioCommandValidator(IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// ASSIGN TAG TO SCENARIO
// ───────────────────────────────────────────────

public sealed record AssignTagToScenarioCommand(Guid ScenarioId, Guid TagId) : IRequest;

public sealed class AssignTagToScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<AssignTagToScenarioCommand>
{
    public async Task Handle(AssignTagToScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.AssignTagAsync(cmd.ScenarioId, cmd.TagId, ct);
}

public sealed class AssignTagToScenarioCommandValidator : AbstractValidator<AssignTagToScenarioCommand>
{
    public AssignTagToScenarioCommandValidator(
        IScenarioRepository scenarioRepo,
        ITagRepository tagRepo)
    {
        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

        RuleFor(x => x.TagId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await tagRepo.ExistsAsync(id, ct))
            .WithMessage("Tag o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// REMOVE TAG FROM SCENARIO
// ───────────────────────────────────────────────

public sealed record RemoveTagFromScenarioCommand(Guid ScenarioId, Guid TagId) : IRequest;

public sealed class RemoveTagFromScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<RemoveTagFromScenarioCommand>
{
    public async Task Handle(RemoveTagFromScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.RemoveTagAsync(cmd.ScenarioId, cmd.TagId, ct);
}

public sealed class RemoveTagFromScenarioCommandValidator : AbstractValidator<RemoveTagFromScenarioCommand>
{
    public RemoveTagFromScenarioCommandValidator(IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

        RuleFor(x => x.TagId).NotEmpty();
    }
}

// ───────────────────────────────────────────────
// ASSIGN RULE TO SCENARIO
// ───────────────────────────────────────────────

public sealed record AssignRuleToScenarioCommand(Guid ScenarioId, Guid RuleId) : IRequest;

public sealed class AssignRuleToScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<AssignRuleToScenarioCommand>
{
    public async Task Handle(AssignRuleToScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.AssignRuleAsync(cmd.ScenarioId, cmd.RuleId, ct);
}

public sealed class AssignRuleToScenarioCommandValidator : AbstractValidator<AssignRuleToScenarioCommand>
{
    public AssignRuleToScenarioCommandValidator(
        IScenarioRepository scenarioRepo,
        IRuleDefinitionRepository ruleRepo)
    {
        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

        RuleFor(x => x.RuleId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await ruleRepo.ExistsAsync(id, ct))
            .WithMessage("Reguła o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// REMOVE RULE FROM SCENARIO
// ───────────────────────────────────────────────

public sealed record RemoveRuleFromScenarioCommand(Guid ScenarioId, Guid RuleId) : IRequest;

public sealed class RemoveRuleFromScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<RemoveRuleFromScenarioCommand>
{
    public async Task Handle(RemoveRuleFromScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.RemoveRuleAsync(cmd.ScenarioId, cmd.RuleId, ct);
}

public sealed class RemoveRuleFromScenarioCommandValidator : AbstractValidator<RemoveRuleFromScenarioCommand>
{
    public RemoveRuleFromScenarioCommandValidator(IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

        RuleFor(x => x.RuleId).NotEmpty();
    }
}