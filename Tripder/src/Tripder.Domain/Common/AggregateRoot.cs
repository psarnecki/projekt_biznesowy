namespace Tripder.Domain.Common;

// AggregateRoot to specjalny typ Encji (dlatego z niej dziedziczy).
// W architekturze DDD to "szef" grupy obiektów. Repozytoria (np. zapis do bazy)
// robimy TYLKO dla Aggregate Rootów.
// Przykład: Attraction będzie AggregateRootem, ale jej Scenario będzie zwykłym Entity.
// Zapisując Attraction, automatycznie zapiszemy jej scenariusze.
public abstract class AggregateRoot : Entity
{
}
