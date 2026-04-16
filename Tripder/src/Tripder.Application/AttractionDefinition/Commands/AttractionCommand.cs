using FluentValidation;
using MediatR;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.Common;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.ValueObjects;
using IDomainAttractionRepository = Tripder.Domain.AttractionDefinition.Repositories.IAttractionRepository;

namespace Tripder.Application.AttractionDefinition.Commands;

// Create attraction
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
    IDomainAttractionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<CreateAttractionCommand, Guid>
{
    public async Task<Guid> Handle(CreateAttractionCommand cmd, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        
        var attraction = new Attraction(
            id,
            cmd.Name,
            cmd.CategoryId,
            new Location(cmd.Latitude, cmd.Longitude, cmd.LocationName),
            cmd.Capacity,
            cmd.CatalogFrom,
            cmd.CatalogTo
        );

        await domainRepo.AddAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);

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

// Update attraction
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
    IDomainAttractionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<UpdateAttractionCommand>
{
    public async Task Handle(UpdateAttractionCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.Id, ct)
            ?? throw new KeyNotFoundException($"Attraction {cmd.Id} not found.");

        attraction.Update(
            cmd.Name,
            cmd.CategoryId,
            new Location(cmd.Latitude, cmd.Longitude, cmd.LocationName),
            cmd.Capacity,
            cmd.CatalogFrom,
            cmd.CatalogTo
        );

        await domainRepo.UpdateAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);
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

// Delete attraction
public sealed record DeleteAttractionCommand(Guid Id) : IRequest;

public sealed class DeleteAttractionCommandHandler(
    IDomainAttractionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<DeleteAttractionCommand>
{
    public async Task Handle(DeleteAttractionCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.Id, ct);
        if (attraction is not null)
        {
            await domainRepo.DeleteAsync(attraction, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
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

// Publish attraction (Draft → Catalog)
public sealed record PublishAttractionCommand(Guid AttractionId) : IRequest;

public sealed class PublishAttractionCommandHandler(
    IDomainAttractionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<PublishAttractionCommand>
{
    public async Task Handle(PublishAttractionCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.AttractionId, ct)
            ?? throw new KeyNotFoundException($"Attraction {cmd.AttractionId} not found.");

        attraction.Publish();

        await domainRepo.UpdateAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);
    }
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

// Archive attraction (Catalog/Internal → Archived)
public sealed record ArchiveAttractionCommand(Guid AttractionId) : IRequest;

public sealed class ArchiveAttractionCommandHandler(
    IDomainAttractionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<ArchiveAttractionCommand>
{
    public async Task Handle(ArchiveAttractionCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.AttractionId, ct)
            ?? throw new KeyNotFoundException($"Attraction {cmd.AttractionId} not found.");

        attraction.Archive();

        await domainRepo.UpdateAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);
    }
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

// Set catalog window (globalny wyłącznik awaryjny)

public sealed record SetAttractionCatalogWindowCommand(
    Guid AttractionId,
    DateOnly? CatalogFrom,
    DateOnly? CatalogTo
) : IRequest;

public sealed class SetAttractionCatalogWindowCommandHandler(
    IDomainAttractionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<SetAttractionCatalogWindowCommand>
{
    public async Task Handle(SetAttractionCatalogWindowCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.AttractionId, ct)
            ?? throw new KeyNotFoundException($"Attraction {cmd.AttractionId} not found.");

        attraction.SetCatalogWindow(cmd.CatalogFrom, cmd.CatalogTo);

        await domainRepo.UpdateAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);
    }
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

// Add tag to attraction (by tag name)
public sealed record AddTagToAttractionCommand(Guid AttractionId, string TagName) : IRequest;

public sealed class AddTagToAttractionCommandHandler(
    IDomainAttractionRepository domainRepo,
    ITagRepository tagRepo,
    IUnitOfWork uow
) : IRequestHandler<AddTagToAttractionCommand>
{
    public async Task Handle(AddTagToAttractionCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.AttractionId, ct)
            ?? throw new KeyNotFoundException($"Attraction {cmd.AttractionId} not found.");

        var tagId = await tagRepo.GetOrCreateByNameAsync(cmd.TagName, ct);
        var tag = new Tag(tagId, cmd.TagName);
        
        attraction.AddTag(tag);

        await domainRepo.UpdateAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);
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

// Remove tag from attraction (by tag name)
public sealed record RemoveTagFromAttractionCommand(Guid AttractionId, string TagName) : IRequest;

public sealed class RemoveTagFromAttractionCommandHandler(
    IDomainAttractionRepository domainRepo,
    ITagRepository tagRepo,
    IUnitOfWork uow
) : IRequestHandler<RemoveTagFromAttractionCommand>
{
    public async Task Handle(RemoveTagFromAttractionCommand cmd, CancellationToken ct)
    {
        var tagId = await tagRepo.GetIdByNameAsync(cmd.TagName, ct);
        if (tagId.HasValue)
        {
            var attraction = await domainRepo.GetByIdAsync(cmd.AttractionId, ct)
                ?? throw new KeyNotFoundException($"Attraction {cmd.AttractionId} not found.");

            attraction.RemoveTag(tagId.Value);

            await domainRepo.UpdateAsync(attraction, ct);
            await uow.SaveChangesAsync(ct);
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

// Attach rule to attraction
public sealed record AttachRuleToAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public sealed class AttachRuleToAttractionCommandHandler(
    IDomainAttractionRepository domainRepo,
    Tripder.Domain.AttractionDefinition.Repositories.IRuleDefinitionRepository domainRuleRepo,
    IUnitOfWork uow
) : IRequestHandler<AttachRuleToAttractionCommand>
{
    public async Task Handle(AttachRuleToAttractionCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.AttractionId, ct)
            ?? throw new KeyNotFoundException($"Attraction {cmd.AttractionId} not found.");

        var rule = await domainRuleRepo.GetByIdAsync(cmd.RuleId, ct)
            ?? throw new KeyNotFoundException($"Rule {cmd.RuleId} not found.");

        attraction.AddRule(rule);

        await domainRepo.UpdateAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);
    }
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

// Detach rule from attraction
public sealed record DetachRuleFromAttractionCommand(Guid AttractionId, Guid RuleId) : IRequest;

public sealed class DetachRuleFromAttractionCommandHandler(
    IDomainAttractionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<DetachRuleFromAttractionCommand>
{
    public async Task Handle(DetachRuleFromAttractionCommand cmd, CancellationToken ct)
    {
        var attraction = await domainRepo.GetByIdAsync(cmd.AttractionId, ct)
            ?? throw new KeyNotFoundException($"Attraction {cmd.AttractionId} not found.");

        attraction.RemoveRule(cmd.RuleId);

        await domainRepo.UpdateAsync(attraction, ct);
        await uow.SaveChangesAsync(ct);
    }
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