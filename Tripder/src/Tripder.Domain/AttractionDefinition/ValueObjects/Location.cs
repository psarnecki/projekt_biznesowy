using Tripder.Domain.Common;

namespace Tripder.Domain.AttractionDefinition.ValueObjects;

// Location to klasyczny Value Object (Obiekt Wartości) w podejściu DDD.
// Dlaczego to nie jest Entity? Bo lokalizacja nie ma swojej "tożsamości" (ID). 
// Dwie lokalizacje z tymi samymi współrzędnymi to z biznesowego punktu widzenia to samo miejsce.
public class Location : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string LocationName { get; private set; } = string.Empty;

    // pusty konstruktor tylko dla EF / serializacji — normalnie i tak tworzysz przez ten drugi z walidacją
    private Location()
    {
    }

    // Konstruktor wymusza walidację przy samym tworzeniu obiektu.
    // Dzięki temu obiekt Location NIGDY nie znajdzie się w niepoprawnym stanie.
    // Nie da się tu wrzucić współrzędnych z kosmosu (np. Latitude = 900), bo system od razu rzuci błędem.
    public Location(double latitude, double longitude, string locationName)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude musi być w przedziale od -90 do 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude musi być w przedziale od -180 do 180.");

        if (string.IsNullOrWhiteSpace(locationName))
            throw new ArgumentException("Nazwa lokalizacji nie może być pusta.", nameof(locationName));

        Latitude = latitude;
        Longitude = longitude;
        LocationName = locationName;
    }

    // Nadpisujemy tę metodę z bazowego ValueObject, żeby system wiedział, 
    // po jakich polach ma sprawdzać równość dwóch lokalizacji.
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
        yield return LocationName;
    }
}
