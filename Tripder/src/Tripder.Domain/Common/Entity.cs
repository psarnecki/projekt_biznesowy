namespace Tripder.Domain.Common;

// Baza dla wszystkich obiektów w naszym systemie, które mają własną tożsamość (ID).
// Używamy typu Guid, żeby móc generować ID od razu w kodzie (Guid.NewGuid()),
// bez czekania na to, aż baza danych (PostgreSQL) nada numer ID.
// Zwróć uwagę na 'protected init' - ID nadajemy raz przy tworzeniu i nigdy go nie zmieniamy.
public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
}
