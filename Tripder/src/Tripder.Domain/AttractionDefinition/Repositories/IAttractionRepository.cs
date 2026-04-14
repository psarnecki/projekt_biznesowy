using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.AttractionDefinition.Enums;

namespace Tripder.Domain.AttractionDefinition.Repositories;

public interface IAttractionRepository
{
    Task<Attraction?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Attraction>> GetAllAsync(AttractionState? state = null, CancellationToken ct = default);
    Task AddAsync(Attraction attraction, CancellationToken ct = default);
    Task UpdateAsync(Attraction attraction, CancellationToken ct = default);
    Task DeleteAsync(Attraction attraction, CancellationToken ct = default);
}
