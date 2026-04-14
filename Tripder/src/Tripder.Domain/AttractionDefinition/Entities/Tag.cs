namespace Tripder.Domain.AttractionDefinition.Entities;
public class Tag
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Tag() { }

    public Tag(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}