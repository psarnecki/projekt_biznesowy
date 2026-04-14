namespace Tripder.Domain.AttractionDefinition.Entities;


public class Image
{
    public Guid Id { get; private set; }
    public Guid ScenarioId { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public int OrderIndex { get; private set; }

    private Image() { }

    public Image(Guid id, Guid scenarioId, string url, int orderIndex)
    {
        Id = id;
        ScenarioId = scenarioId;
        Url = url;
        OrderIndex = orderIndex;
    }
}