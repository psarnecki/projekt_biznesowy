using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Domain.AttractionDefinition.Entities;

public class RuleDefinition
{
    public Guid Id { get; private set; }
    public RuleType RuleType { get; private set; }
    public RuleEffect Effect { get; private set; }
    public int Priority { get; private set; }
    public TimeOnly? TimeFrom { get; private set; }
    public TimeOnly? TimeTo { get; private set; }
    public DateOnly? DateFrom { get; private set; }
    public DateOnly? DateTo { get; private set; }
    public string? Params { get; private set; }

    private readonly List<DayOfWeekEntry> _days = new();
    public IReadOnlyList<DayOfWeekEntry> Days => _days.AsReadOnly();

    private RuleDefinition() { }

    public RuleDefinition(
        Guid id,
        RuleType ruleType,
        RuleEffect effect,
        int priority,
        TimeOnly? timeFrom = null,
        TimeOnly? timeTo = null,
        DateOnly? dateFrom = null,
        DateOnly? dateTo = null,
        string? @params = null)
    {
        Id = id;
        RuleType = ruleType;
        Effect = effect;
        Priority = priority;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Params = @params;
    }

    public void AddDay(DayOfWeekEntry day)
    {
        if (!_days.Any(d => d.Id == day.Id))
            _days.Add(day);
    }

    public void Update(RuleType ruleType, RuleEffect effect, int priority, TimeOnly? timeFrom, TimeOnly? timeTo, DateOnly? dateFrom, DateOnly? dateTo, string? @params)
    {
        RuleType = ruleType;
        Effect = effect;
        Priority = priority;
        TimeFrom = timeFrom;
        TimeTo = timeTo;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Params = @params;
    }

    /// Evaluates whether this rule is active for a given date and time
    public bool IsActiveFor(DateOnly date, TimeOnly time)
    {
        if (TimeFrom.HasValue && time < TimeFrom.Value) return false;
        if (TimeTo.HasValue && time > TimeTo.Value) return false;
        if (DateFrom.HasValue && date < DateFrom.Value) return false;
        if (DateTo.HasValue && date > DateTo.Value) return false;

        if (_days.Count > 0)
        {
            var dayName = date.DayOfWeek.ToString();
            if (!_days.Any(d => d.Name.Equals(dayName, StringComparison.OrdinalIgnoreCase)))
                return false;
        }

        return true;
    }
}