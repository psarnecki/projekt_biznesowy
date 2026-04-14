using FluentValidation;
using MediatR;
using Tripder.Application.AttractionDefinition.Repositories;

namespace Tripder.Application.AttractionDefinition.Commands;

// ───────────────────────────────────────────────
// ADD SCENARIO
// ───────────────────────────────────────────────

public sealed record AddScenarioCommand(
    Guid AttractionId,
    string Name,
    string Description,
    int DurationMinutes
) : IRequest<Guid>;

public sealed class AddScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<AddScenarioCommand, Guid>
{
    public async Task<Guid> Handle(AddScenarioCommand cmd, CancellationToken ct)
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

public sealed class AddScenarioCommandValidator : AbstractValidator<AddScenarioCommand>
{
    public AddScenarioCommandValidator(IAttractionRepository attractionRepo)
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
// UPDATE SCENARIO
// ───────────────────────────────────────────────

public sealed record UpdateScenarioCommand(
    Guid AttractionId,
    Guid ScenarioId,
    string Name,
    string Description,
    int DurationMinutes
) : IRequest;

public sealed class UpdateScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<UpdateScenarioCommand>
{
    public async Task Handle(UpdateScenarioCommand cmd, CancellationToken ct)
    {
        await scenarioRepo.UpdateAsync(new UpdateScenarioData(
            cmd.ScenarioId,
            cmd.Name,
            cmd.Description,
            cmd.DurationMinutes
        ), ct);
    }
}

public sealed class UpdateScenarioCommandValidator : AbstractValidator<UpdateScenarioCommand>
{
    public UpdateScenarioCommandValidator(
        IAttractionRepository attractionRepo,
        IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

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
// REMOVE SCENARIO
// ───────────────────────────────────────────────

public sealed record RemoveScenarioCommand(Guid AttractionId, Guid ScenarioId) : IRequest;

public sealed class RemoveScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<RemoveScenarioCommand>
{
    public async Task Handle(RemoveScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.DeleteAsync(cmd.ScenarioId, ct);
}

public sealed class RemoveScenarioCommandValidator : AbstractValidator<RemoveScenarioCommand>
{
    public RemoveScenarioCommandValidator(
        IAttractionRepository attractionRepo,
        IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// PUBLISH SCENARIO (Draft → Catalog)
// ───────────────────────────────────────────────

public sealed record PublishScenarioCommand(Guid AttractionId, Guid ScenarioId) : IRequest;

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

public sealed record ArchiveScenarioCommand(Guid AttractionId, Guid ScenarioId) : IRequest;

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
// ADD TAG TO SCENARIO (by tag name)
// ───────────────────────────────────────────────

public sealed record AddTagToScenarioCommand(Guid AttractionId, Guid ScenarioId, string TagName) : IRequest;

public sealed class AddTagToScenarioCommandHandler(
    IScenarioRepository scenarioRepo,
    ITagRepository tagRepo
) : IRequestHandler<AddTagToScenarioCommand>
{
    public async Task Handle(AddTagToScenarioCommand cmd, CancellationToken ct)
    {
        var tagId = await tagRepo.GetOrCreateByNameAsync(cmd.TagName, ct);
        await scenarioRepo.AssignTagAsync(cmd.ScenarioId, tagId, ct);
    }
}

public sealed class AddTagToScenarioCommandValidator : AbstractValidator<AddTagToScenarioCommand>
{
    public AddTagToScenarioCommandValidator(
        IAttractionRepository attractionRepo,
        IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

        RuleFor(x => x.TagName)
            .NotEmpty().WithMessage("Nazwa tagu jest wymagana.")
            .MaximumLength(50);
    }
}

// ───────────────────────────────────────────────
// REMOVE TAG FROM SCENARIO (by tag name)
// ───────────────────────────────────────────────

public sealed record RemoveTagFromScenarioCommand(Guid AttractionId, Guid ScenarioId, string TagName) : IRequest;

public sealed class RemoveTagFromScenarioCommandHandler(
    IScenarioRepository scenarioRepo,
    ITagRepository tagRepo
) : IRequestHandler<RemoveTagFromScenarioCommand>
{
    public async Task Handle(RemoveTagFromScenarioCommand cmd, CancellationToken ct)
    {
        var tagId = await tagRepo.GetIdByNameAsync(cmd.TagName, ct);
        if (tagId.HasValue)
        {
            await scenarioRepo.RemoveTagAsync(cmd.ScenarioId, tagId.Value, ct);
        }
    }
}

public sealed class RemoveTagFromScenarioCommandValidator : AbstractValidator<RemoveTagFromScenarioCommand>
{
    public RemoveTagFromScenarioCommandValidator(
        IAttractionRepository attractionRepo,
        IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

        RuleFor(x => x.TagName)
            .NotEmpty().WithMessage("Nazwa tagu jest wymagana.");
    }
}

// ───────────────────────────────────────────────
// ATTACH RULE TO SCENARIO
// ───────────────────────────────────────────────

public sealed record AttachRuleToScenarioCommand(Guid AttractionId, Guid ScenarioId, Guid RuleId) : IRequest;

public sealed class AttachRuleToScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<AttachRuleToScenarioCommand>
{
    public async Task Handle(AttachRuleToScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.AssignRuleAsync(cmd.ScenarioId, cmd.RuleId, ct);
}

public sealed class AttachRuleToScenarioCommandValidator : AbstractValidator<AttachRuleToScenarioCommand>
{
    public AttachRuleToScenarioCommandValidator(
        IAttractionRepository attractionRepo,
        IScenarioRepository scenarioRepo,
        IRuleDefinitionRepository ruleRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

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
// DETACH RULE FROM SCENARIO
// ───────────────────────────────────────────────

public sealed record DetachRuleFromScenarioCommand(Guid AttractionId, Guid ScenarioId, Guid RuleId) : IRequest;

public sealed class DetachRuleFromScenarioCommandHandler(
    IScenarioRepository scenarioRepo
) : IRequestHandler<DetachRuleFromScenarioCommand>
{
    public async Task Handle(DetachRuleFromScenarioCommand cmd, CancellationToken ct)
        => await scenarioRepo.RemoveRuleAsync(cmd.ScenarioId, cmd.RuleId, ct);
}

public sealed class DetachRuleFromScenarioCommandValidator : AbstractValidator<DetachRuleFromScenarioCommand>
{
    public DetachRuleFromScenarioCommandValidator(
        IAttractionRepository attractionRepo,
        IScenarioRepository scenarioRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.ScenarioId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await scenarioRepo.ExistsAsync(id, ct))
            .WithMessage("Scenariusz o podanym Id nie istnieje.");

        RuleFor(x => x.RuleId).NotEmpty();
    }
}