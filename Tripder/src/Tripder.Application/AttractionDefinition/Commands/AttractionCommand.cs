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
    IAttractionRepository attractionRepo
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
// UPDATE ATTRACTION
// ───────────────────────────────────────────────

public sealed record UpdateAttractionCommand(
    Guid Id,
    string Name,
    Guid CategoryId,
    string LocationName,
    float Latitude,
    float Longitude,
    int? Capacity,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo
) : IRequest;

public sealed class UpdateAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<UpdateAttractionCommand>
{
    public async Task Handle(UpdateAttractionCommand cmd, CancellationToken ct)
    {
        await attractionRepo.UpdateAsync(new UpdateAttractionData(
            cmd.Id,
            cmd.Name,
            cmd.CategoryId,
            cmd.LocationName,
            cmd.Latitude,
            cmd.Longitude,
            cmd.Capacity,
            cmd.CatalogFrom,
            cmd.CatalogTo
        ), ct);
    }
}

public sealed class UpdateAttractionCommandValidator : AbstractValidator<UpdateAttractionCommand>
{
    public UpdateAttractionCommandValidator(
        IAttractionRepository attractionRepo,
        ICategoryRepository categoryRepo)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa atrakcji jest wymagana.")
            .MaximumLength(200).WithMessage("Nazwa atrakcji nie może przekraczać 200 znaków.")
            .MustAsync(async (cmd, name, ct) => !await attractionRepo.NameExistsAsync(name, cmd.Id, ct))
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
// DELETE ATTRACTION
// ───────────────────────────────────────────────

public sealed record DeleteAttractionCommand(Guid Id) : IRequest;

public sealed class DeleteAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<DeleteAttractionCommand>
{
    public async Task Handle(DeleteAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.DeleteAsync(cmd.Id, ct);
}

public sealed class DeleteAttractionCommandValidator : AbstractValidator<DeleteAttractionCommand>
{
    public DeleteAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");
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
// ADD TAG TO ATTRACTION (by tag name)
// ───────────────────────────────────────────────

public sealed record AddTagToAttractionCommand(Guid AttractionId, string TagName) : IRequest;

public sealed class AddTagToAttractionCommandHandler(
    IAttractionRepository attractionRepo,
    ITagRepository tagRepo
) : IRequestHandler<AddTagToAttractionCommand>
{
    public async Task Handle(AddTagToAttractionCommand cmd, CancellationToken ct)
    {
        var tagId = await tagRepo.GetOrCreateByNameAsync(cmd.TagName, ct);
        await attractionRepo.AssignTagAsync(cmd.AttractionId, tagId, ct);
    }
}

public sealed class AddTagToAttractionCommandValidator : AbstractValidator<AddTagToAttractionCommand>
{
    public AddTagToAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.TagName)
            .NotEmpty().WithMessage("Nazwa tagu jest wymagana.")
            .MaximumLength(50);
    }
}

// ───────────────────────────────────────────────
// REMOVE TAG FROM ATTRACTION (by tag name)
// ───────────────────────────────────────────────

public sealed record RemoveTagFromAttractionCommand(Guid AttractionId, string TagName) : IRequest;

public sealed class RemoveTagFromAttractionCommandHandler(
    IAttractionRepository attractionRepo,
    ITagRepository tagRepo
) : IRequestHandler<RemoveTagFromAttractionCommand>
{
    public async Task Handle(RemoveTagFromAttractionCommand cmd, CancellationToken ct)
    {
        var tagId = await tagRepo.GetIdByNameAsync(cmd.TagName, ct);
        if (tagId.HasValue)
        {
            await attractionRepo.RemoveTagAsync(cmd.AttractionId, tagId.Value, ct);
        }
    }
}

public sealed class RemoveTagFromAttractionCommandValidator : AbstractValidator<RemoveTagFromAttractionCommand>
{
    public RemoveTagFromAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.TagName)
            .NotEmpty().WithMessage("Nazwa tagu jest wymagana.");
    }
}

// ───────────────────────────────────────────────
// ATTACH RULE TO ATTRACTION
// ───────────────────────────────────────────────

public sealed record AttachRuleToAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public sealed class AttachRuleToAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<AttachRuleToAttractionCommand>
{
    public async Task Handle(AttachRuleToAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.AssignRuleAsync(cmd.AttractionId, cmd.RuleId, ct);
}

public sealed class AttachRuleToAttractionCommandValidator : AbstractValidator<AttachRuleToAttractionCommand>
{
    public AttachRuleToAttractionCommandValidator(
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
// DETACH RULE FROM ATTRACTION
// ───────────────────────────────────────────────

public sealed record DetachRuleFromAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public sealed class DetachRuleFromAttractionCommandHandler(
    IAttractionRepository attractionRepo
) : IRequestHandler<DetachRuleFromAttractionCommand>
{
    public async Task Handle(DetachRuleFromAttractionCommand cmd, CancellationToken ct)
        => await attractionRepo.RemoveRuleAsync(cmd.AttractionId, cmd.RuleId, ct);
}

public sealed class DetachRuleFromAttractionCommandValidator : AbstractValidator<DetachRuleFromAttractionCommand>
{
    public DetachRuleFromAttractionCommandValidator(IAttractionRepository attractionRepo)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .MustAsync(async (id, ct) => await attractionRepo.ExistsAsync(id, ct))
            .WithMessage("Atrakcja o podanym Id nie istnieje.");

        RuleFor(x => x.RuleId).NotEmpty();
    }
}