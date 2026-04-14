using Tripder.Domain.AttractionDefinition.Enums;
using Tripder.Domain.AttractionDefinition.ValueObjects;
using Tripder.Domain.Common;

namespace Tripder.Domain.AttractionDefinition.Entities;

public class Attraction : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public Location Location { get; private set; } = null!;
    public int? Capacity { get; private set; }
    public DateOnly? CatalogFrom { get; private set; }
    public DateOnly? CatalogTo { get; private set; }
    public AttractionState State { get; private set; }

    private readonly List<Scenario> _scenarios = new();
    public IReadOnlyList<Scenario> Scenarios => _scenarios.AsReadOnly();

    private readonly List<Tag> _tags = new();
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();

    private readonly List<RuleDefinition> _rules = new();
    public IReadOnlyList<RuleDefinition> Rules => _rules.AsReadOnly();

    private Attraction() { }

    public Attraction(
        Guid id,
        string name,
        Guid categoryId,
        Location location,
        int? capacity = null,
        DateOnly? catalogFrom = null,
        DateOnly? catalogTo = null)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
        Location = location;
        Capacity = capacity;
        CatalogFrom = catalogFrom;
        CatalogTo = catalogTo;
        State = AttractionState.Draft;
    }

    public void Publish()
    {
        if (State != AttractionState.Draft)
            throw new InvalidOperationException("Only Draft attractions can be published.");
        State = AttractionState.Catalog;
    }

    public void MakeInternal()
    {
        State = AttractionState.Internal;
    }

    public void Archive()
    {
        if (State == AttractionState.Archived)
            throw new InvalidOperationException("Attraction is already archived.");
        State = AttractionState.Archived;
    }

    public void SetCatalogWindow(DateOnly? from, DateOnly? to)
    {
        CatalogFrom = from;
        CatalogTo = to;
    }

    public void AddScenario(Scenario scenario)
    {
        if (scenario.AttractionId != Id)
            throw new InvalidOperationException("Scenario does not belong to this attraction.");
        _scenarios.Add(scenario);
    }

    public void AddTag(Tag tag)
    {
        if (!_tags.Any(t => t.Id == tag.Id))
            _tags.Add(tag);
    }

    public void RemoveTag(Guid tagId)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is not null) _tags.Remove(tag);
    }

    public void AddRule(RuleDefinition rule)
    {
        if (!_rules.Any(r => r.Id == rule.Id))
            _rules.Add(rule);
    }

    public void RemoveRule(Guid ruleId)
    {
        var rule = _rules.FirstOrDefault(r => r.Id == ruleId);
        if (rule is not null) _rules.Remove(rule);
    }
    
    /// Returns true if the attraction's global catalog window is active on a given date.
    public bool IsVisibleOnDate(DateOnly date)
    {
        if (State != AttractionState.Catalog) return false;
        if (CatalogFrom.HasValue && date < CatalogFrom.Value) return false;
        if (CatalogTo.HasValue && date > CatalogTo.Value) return false;
        return true;
    }
    
    /// Haversine distance in km to a point.
    public double DistanceKmTo(double lat, double lon)
    {
        const double R = 6371;
        var dLat = ToRad(lat - Location.Latitude);
        var dLon = ToRad(lon - Location.Longitude);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(Location.Latitude)) * Math.Cos(ToRad(lat)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;
}