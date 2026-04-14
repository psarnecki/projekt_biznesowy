namespace Tripder.Domain.AttractionDefiniton.Entities;


public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Category() { }

    public Category(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}