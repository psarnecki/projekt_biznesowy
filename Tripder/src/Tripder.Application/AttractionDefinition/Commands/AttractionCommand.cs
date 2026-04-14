using FluentValidation;
using MediatR;
using Tripder.Application.AttractionDefinition.Repositories;

namespace Tripder.Application.AttractionDefinition.Commands;

// ───────────────────────────────────────────────
// CREATE ATTRACTION
// ───────────────────────────────────────────────

public sealed record CreateAttractionCommand(
    string Name,
    Guid CategoryId,
    string LocationName,
    float Latitude,
    float Longitude,
    int? Capacity,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo
) : IRequest<Guid>;

public sealed class CreateAttractionCommandHandler(
    IAttractionRepository attractionRepo,
    ICategoryRepository categoryRepo
) : IRequestHandler<CreateAttractionCommand, Guid>
{
    public async Task<Guid> Handle(CreateAttractionCommand cmd, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        await attractionRepo.AddAsync(new NewAttractionData(
            id,
            cmd.Name,
            cmd.CategoryId,
            cmd.LocationName,
            cmd.Latitude,
            cmd.Longitude,
            cmd.Capacity,
            cmd.CatalogFrom,
            cmd.CatalogTo
        ), ct);

        return id;
    }
}

public sealed class CreateAttractionCommandValidator : AbstractValidator<CreateAttractionCommand>
{
    public CreateAttractionCommandValidator(
        IAttractionRepository attractionRepo,
        ICategoryRepository categoryRepo)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa atrakcji jest wymagana.")
            .MaximumLength(200).WithMessage("Nazwa atrakcji nie może przekraczać 200 znaków.")
            .MustAsync(async (name, ct) => !await attractionRepo.NameExistsAsync(name, ct: ct))
            .WithMessage("Atrakcja o podanej nazwie już istnieje.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Kategoria jest wymagana.")
            .MustAsync(async (id, ct) => await categoryRepo.ExistsAsync(id, ct))
            .WithMessage("Podana kategoria nie istnieje.");

        RuleFor(x => x.LocationName)
            .NotEmpty().WithMessage("Nazwa lokalizacji jest wymagana.")
            .MaximumLength(300);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90f, 90f).WithMessage("Szerokość geograficzna musi być w zakresie -90 do 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180f, 180f).WithMessage("Długość geograficzna musi być w zakresie -180 do 180.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).When(x => x.Capacity.HasValue)
            .WithMessage("Pojemność musi być wartością dodatnią.");

        RuleFor(x => x.CatalogTo)
            .GreaterThan(x => x.CatalogFrom).When(x => x.CatalogFrom.HasValue && x.CatalogTo.HasValue)
            .WithMessage("Data końca katalogu musi być późniejsza niż data początku.");
    }
}

// ───────────────────────────────────────────────
// PUBLISH ATTRACTION (Draft → Catalog)
// ───────────────────────────────────────────────

public sealed record PublishAttractionCommand(Guid AttractionId) : IRequest;

public sealed class PublishAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<PublishAttractionCommand>
{
    public async Task Handle(PublishAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.UpdateStateAsync(cmd.AttractionId, "Catalog", ct);
}

public sealed class PublishAttractionCommandValidator : AbstractValidator<PublishAttractionCommand>
{
    public PublishAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// ARCHIVE ATTRACTION (Catalog/Internal → Archived)
// ───────────────────────────────────────────────

public sealed record ArchiveAttractionCommand(Guid AttractionId) : IRequest;

public sealed class ArchiveAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<ArchiveAttractionCommand>
{
    public async Task Handle(ArchiveAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.UpdateStateAsync(cmd.AttractionId, "Archived", ct);
}

public sealed class ArchiveAttractionCommandValidator : AbstractValidator<ArchiveAttractionCommand>
{
    public ArchiveAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// SET CATALOG WINDOW (globalny wyłącznik awaryjny)
// ───────────────────────────────────────────────

public sealed record SetAttractionCatalogWindowCommand(
    Guid AttractionId,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo
) : IRequest;

public sealed class SetAttractionCatalogWindowCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<SetAttractionCatalogWindowCommand>
{
    public async Task Handle(SetAttractionCatalogWindowCommand cmd, CancellationToken ct)
        => await attractionRepo.UpdateCatalogWindowAsync(cmd.AttractionId, cmd.CatalogFrom, cmd.CatalogTo, ct);
}

public sealed class SetAttractionCatalogWindowCommandValidator : AbstractValidator<SetAttractionCatalogWindowCommand>
{
    public SetAttractionCatalogWindowCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.CatalogTo)
            .GreaterThan(x => x.CatalogFrom).When(x => x.CatalogFrom.HasValue && x.CatalogTo.HasValue)
            .WithMessage("Data końca katalogu musi być późniejsza niż data początku.");
    }
}

// ───────────────────────────────────────────────
// ASSIGN TAG TO ATTRACTION
// ───────────────────────────────────────────────

public sealed record AssignTagToAttractionCommand(Guid AttractionId, Guid TagId) : IRequest;

public sealed class AssignTagToAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<AssignTagToAttractionCommand>
{
    public async Task Handle(AssignTagToAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.AssignTagAsync(cmd.AttractionId, cmd.TagId, ct);
}

public sealed class AssignTagToAttractionCommandValidator : AbstractValidator<AssignTagToAttractionCommand>
{
    public AssignTagToAttractionCommandValidator(
        IAttractionRepository attractionRepo,
        ITagRepository tagRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.TagId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await tagRepo.ExistsAsync(id, ct))
            .WithMessage("Tag o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// REMOVE TAG FROM ATTRACTION
// ───────────────────────────────────────────────

public sealed record RemoveTagFromAttractionCommand(Guid AttractionId, Guid TagId) : IRequest;

public sealed class RemoveTagFromAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<RemoveTagFromAttractionCommand>
{
    public async Task Handle(RemoveTagFromAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.RemoveTagAsync(cmd.AttractionId, cmd.TagId, ct);
}

public sealed class RemoveTagFromAttractionCommandValidator : AbstractValidator<RemoveTagFromAttractionCommand>
{
    public RemoveTagFromAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.TagId).NotEmpty();
    }
}

// ───────────────────────────────────────────────
// ASSIGN RULE TO ATTRACTION
// ───────────────────────────────────────────────

public sealed record AssignRuleToAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public sealed class AssignRuleToAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<AssignRuleToAttractionCommand>
{
    public async Task Handle(AssignRuleToAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.AssignRuleAsync(cmd.AttractionId, cmd.RuleId, ct);
}

public sealed class AssignRuleToAttractionCommandValidator : AbstractValidator<AssignRuleToAttractionCommand>
{
    public AssignRuleToAttractionCommandValidator(
        IAttractionRepository attractionRepo,
        IRuleDefinitionRepository ruleRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.RuleId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await ruleRepo.ExistsAsync(id, ct))
            .WithMessage("Reguła o podanym Id nie istnieje.");
    }
}

// ───────────────────────────────────────────────
// REMOVE RULE FROM ATTRACTION
// ───────────────────────────────────────────────

public sealed record RemoveRuleFromAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public sealed class RemoveRuleFromAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<RemoveRuleFromAttractionCommand>
{
    public async Task Handle(RemoveRuleFromAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.RemoveRuleAsync(cmd.AttractionId, cmd.RuleId, ct);
}

public sealed class RemoveRuleFromAttractionCommandValidator : AbstractValidator<RemoveRuleFromAttractionCommand>
{
    public RemoveRuleFromAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.RuleId).NotEmpty();
    }
}