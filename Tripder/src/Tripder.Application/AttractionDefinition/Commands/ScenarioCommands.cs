using FluentValidation;
using MediatR;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.Common;
using Tripder.Domain.AttractionDefinition.Entities;
using IDomainScenarioRepository = Tripder.Domain.AttractionDefinition.Repositories.IScenarioRepository;

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
    IDomainScenarioRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<AddScenarioCommand, Guid>
{
    public async Task<Guid> Handle(AddScenarioCommand cmd, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        var scenario = new Scenario(
            id,
            cmd.AttractionId,
            cmd.Name,
            cmd.Description,
            cmd.DurationMinutes
        );

        await domainRepo.AddAsync(scenario, ct);
        await uow.SaveChangesAsync(ct);

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
    IDomainScenarioRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<UpdateScenarioCommand>
{
    public async Task Handle(UpdateScenarioCommand cmd, CancellationToken ct)
    {
        var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct)
            ?? throw new KeyNotFoundException($"Scenario {cmd.ScenarioId} not found.");

        scenario.Update(cmd.Name, cmd.Description, cmd.DurationMinutes);

        await domainRepo.UpdateAsync(scenario, ct);
        await uow.SaveChangesAsync(ct);
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
    IDomainScenarioRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<RemoveScenarioCommand>
{
    public async Task Handle(RemoveScenarioCommand cmd, CancellationToken ct)
    {
        var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct);
        if (scenario is not null)
        {
            await domainRepo.DeleteAsync(scenario, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
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
    IDomainScenarioRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<PublishScenarioCommand>
{
    public async Task Handle(PublishScenarioCommand cmd, CancellationToken ct)
    {
        var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct)
            ?? throw new KeyNotFoundException($"Scenario {cmd.ScenarioId} not found.");

        scenario.Publish();

        await domainRepo.UpdateAsync(scenario, ct);
        await uow.SaveChangesAsync(ct);
    }
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
    IDomainScenarioRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<ArchiveScenarioCommand>
{
    public async Task Handle(ArchiveScenarioCommand cmd, CancellationToken ct)
    {
        var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct)
            ?? throw new KeyNotFoundException($"Scenario {cmd.ScenarioId} not found.");

        scenario.Archive();

        await domainRepo.UpdateAsync(scenario, ct);
        await uow.SaveChangesAsync(ct);
    }
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
    IDomainScenarioRepository domainRepo,
    ITagRepository tagRepo,
    IUnitOfWork uow
) : IRequestHandler<AddTagToScenarioCommand>
{
    public async Task Handle(AddTagToScenarioCommand cmd, CancellationToken ct)
    {
        var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct)
            ?? throw new KeyNotFoundException($"Scenario {cmd.ScenarioId} not found.");

        var tagId = await tagRepo.GetOrCreateByNameAsync(cmd.TagName, ct);
        var tag = new Tag(tagId, cmd.TagName);
        
        scenario.AddTag(tag);

        await domainRepo.UpdateAsync(scenario, ct);
        await uow.SaveChangesAsync(ct);
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
    IDomainScenarioRepository domainRepo,
    ITagRepository tagRepo,
    IUnitOfWork uow
) : IRequestHandler<RemoveTagFromScenarioCommand>
{
    public async Task Handle(RemoveTagFromScenarioCommand cmd, CancellationToken ct)
    {
        var tagId = await tagRepo.GetIdByNameAsync(cmd.TagName, ct);
        if (tagId.HasValue)
        {
            var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct)
                ?? throw new KeyNotFoundException($"Scenario {cmd.ScenarioId} not found.");

            scenario.RemoveTag(tagId.Value);

            await domainRepo.UpdateAsync(scenario, ct);
            await uow.SaveChangesAsync(ct);
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
    IDomainScenarioRepository domainRepo,
    Tripder.Domain.AttractionDefinition.Repositories.IRuleDefinitionRepository domainRuleRepo,
    IUnitOfWork uow
) : IRequestHandler<AttachRuleToScenarioCommand>
{
    public async Task Handle(AttachRuleToScenarioCommand cmd, CancellationToken ct)
    {
        var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct)
            ?? throw new KeyNotFoundException($"Scenario {cmd.ScenarioId} not found.");

        var rule = await domainRuleRepo.GetByIdAsync(cmd.RuleId, ct)
            ?? throw new KeyNotFoundException($"Rule {cmd.RuleId} not found.");

        scenario.AddRule(rule);

        await domainRepo.UpdateAsync(scenario, ct);
        await uow.SaveChangesAsync(ct);
    }
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
    IDomainScenarioRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<DetachRuleFromScenarioCommand>
{
    public async Task Handle(DetachRuleFromScenarioCommand cmd, CancellationToken ct)
    {
        var scenario = await domainRepo.GetByIdAsync(cmd.ScenarioId, ct)
            ?? throw new KeyNotFoundException($"Scenario {cmd.ScenarioId} not found.");

        scenario.RemoveRule(cmd.RuleId);

        await domainRepo.UpdateAsync(scenario, ct);
        await uow.SaveChangesAsync(ct);
    }
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