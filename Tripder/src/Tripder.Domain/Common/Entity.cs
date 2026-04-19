namespace Tripder.Domain.Common;

// Base class for entities with a stable Guid identifier
public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
}
