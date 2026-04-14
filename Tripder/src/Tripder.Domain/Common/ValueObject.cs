namespace Tripder.Domain.Common;

// Baza dla obiektów, które NIE MAJĄ własnego ID, a definiują je wyłącznie ich wartości (jak nasz Location).
// Ta klasa zawiera magiczny kod, który pozwala C# automatycznie porównywać dwa obiekty
// po ich polach (właściwościach), zamiast po referencji w pamięci.
public abstract class ValueObject
{
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;
        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
}
