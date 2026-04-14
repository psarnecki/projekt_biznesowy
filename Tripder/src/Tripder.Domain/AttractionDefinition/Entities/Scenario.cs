using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Domain.AttractionDefinition.Entities;

public class Scenario
{
    public Guid Id { get; private set; }
    public Guid AttractionId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int DurationMinutes { get; private set; }
    public ScenarioState State { get; private set; }

    private readonly List<Image> _images = new();
    public IReadOnlyList<Image> Images => _images.AsReadOnly();

    private readonly List<Tag> _tags = new();
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();

    private readonly List<RuleDefinition> _rules = new();
    public IReadOnlyList<RuleDefinition> Rules => _rules.AsReadOnly();

    private Scenario() { }

    public Scenario(
        Guid id,
        Guid attractionId,
        string name,
        string description,
        int durationMinutes)
    {
        Id = id;
        AttractionId = attractionId;
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        State = ScenarioState.Draft;
    }

    public void Publish()
    {
        if (State != ScenarioState.Draft)
            throw new InvalidOperationException("Only Draft scenarios can be published.");
        State = ScenarioState.Catalog;
    }

    public void Archive()
    {
        if (State == ScenarioState.Archived)
            throw new InvalidOperationException("Scenario is already archived.");
        State = ScenarioState.Archived;
    }

    public void Update(string name, string description, int durationMinutes)
    {
        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
    }

    public void AddImage(Image image)
    {
        _images.Add(image);
    }

    public void RemoveImage(Guid imageId)
    {
        var img = _images.FirstOrDefault(i => i.Id == imageId);
        if (img is not null) _images.Remove(img);
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
    
    /// Evaluates scenario availability using its rules (highest priority wins).
    /// Returns null if no rules match (treat as allowed by default).
    public bool IsAvailableAt(DateOnly date, TimeOnly time)
    {
        if (State != ScenarioState.Catalog) return false;

        var matchingRules = _rules
            .Where(r => r.IsActiveFor(date, time))
            .OrderByDescending(r => r.Priority)
            .ToList();

        if (!matchingRules.Any()) return true; // no rules → open by default

        return matchingRules.First().Effect == RuleEffect.Allow;
    }
}