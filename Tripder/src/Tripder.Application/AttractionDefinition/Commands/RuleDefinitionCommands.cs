using FluentValidation;
using MediatR;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.Common;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;
using IDomainRuleDefinitionRepository = Tripder.Domain.AttractionDefinition.Repositories.IRuleDefinitionRepository;

namespace Tripder.Application.AttractionDefinition.Commands;

// Create rule definition
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
    IDomainRuleDefinitionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<CreateRuleDefinitionCommand, Guid>
{
    public async Task<Guid> Handle(CreateRuleDefinitionCommand cmd, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        
        var rule = new RuleDefinition(
            id,
            ParseRuleType(cmd.RuleType),
            ParseRuleEffect(cmd.Effect),
            cmd.Priority,
            cmd.TimeFrom,
            cmd.TimeTo,
            cmd.DateFrom,
            cmd.DateTo,
            cmd.Params
        );

        if (cmd.DayOfWeekIds is { Count: > 0 })
        {
            foreach (var dayId in cmd.DayOfWeekIds)
            {
                var dayName = await domainRepo.GetDayOfWeekNameAsync(dayId, ct)
                    ?? throw new KeyNotFoundException($"DayOfWeekEntry {dayId} not found.");
                rule.AddDay(new DayOfWeekEntry(dayId, dayName));
            }
        }

        await domainRepo.AddAsync(rule, ct);
        await uow.SaveChangesAsync(ct);

        return id;
    }

    private static RuleType ParseRuleType(string value) => Enum.Parse<RuleType>(value, true);
    private static RuleEffect ParseRuleEffect(string value) => Enum.Parse<RuleEffect>(value, true);
}

public sealed class CreateRuleDefinitionCommandValidator : AbstractValidator<CreateRuleDefinitionCommand>
{
    private static readonly string[] ValidRuleTypes = ["Weekly", "Seasonal", "DateException"];
    private static readonly string[] ValidEffects = ["Allow", "Deny"];

    public CreateRuleDefinitionCommandValidator()
    {
        RuleFor(x => x.RuleType)
            .NotEmpty()
            .Must(t => ValidRuleTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"RuleType musi być jedną z wartości: {string.Join(", ", ValidRuleTypes)}.");

        RuleFor(x => x.Effect)
            .NotEmpty()
            .Must(e => ValidEffects.Contains(e, StringComparer.OrdinalIgnoreCase))
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

// Update rule definition
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
    IDomainRuleDefinitionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<UpdateRuleDefinitionCommand>
{
    public async Task Handle(UpdateRuleDefinitionCommand cmd, CancellationToken ct)
    {
        var rule = await domainRepo.GetByIdAsync(cmd.RuleId, ct)
            ?? throw new KeyNotFoundException($"Rule {cmd.RuleId} not found.");

        rule.Update(
            ParseRuleType(cmd.RuleType),
            ParseRuleEffect(cmd.Effect),
            cmd.Priority,
            cmd.TimeFrom,
            cmd.TimeTo,
            cmd.DateFrom,
            cmd.DateTo,
            cmd.Params
        );

        rule.ClearDays();
        if (cmd.DayOfWeekIds is { Count: > 0 })
        {
            foreach (var dayId in cmd.DayOfWeekIds)
            {
                var dayName = await domainRepo.GetDayOfWeekNameAsync(dayId, ct)
                    ?? throw new KeyNotFoundException($"DayOfWeekEntry {dayId} not found.");
                rule.AddDay(new DayOfWeekEntry(dayId, dayName));
            }
        }

        await domainRepo.UpdateAsync(rule, ct);
        await uow.SaveChangesAsync(ct);
    }
    
    private static RuleType ParseRuleType(string value) => Enum.Parse<RuleType>(value, true);
    private static RuleEffect ParseRuleEffect(string value) => Enum.Parse<RuleEffect>(value, true);
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
            .Must(t => ValidRuleTypes.Contains(t, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"RuleType musi być jedną z wartości: {string.Join(", ", ValidRuleTypes)}.");

        RuleFor(x => x.Effect)
            .NotEmpty()
            .Must(e => ValidEffects.Contains(e, StringComparer.OrdinalIgnoreCase))
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

// Delete rule definition
public sealed record DeleteRuleDefinitionCommand(Guid RuleId) : IRequest;

public sealed class DeleteRuleDefinitionCommandHandler(
    IDomainRuleDefinitionRepository domainRepo,
    IUnitOfWork uow
) : IRequestHandler<DeleteRuleDefinitionCommand>
{
    public async Task Handle(DeleteRuleDefinitionCommand cmd, CancellationToken ct)
    {
        var rule = await domainRepo.GetByIdAsync(cmd.RuleId, ct);
        if (rule is not null)
        {
            await domainRepo.DeleteAsync(rule, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
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