namespace Tripder.Domain.AttractionDefiniton.Entities;

public class DayOfWeekEntry
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private DayOfWeekEntry() { }

    public DayOfWeekEntry(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}