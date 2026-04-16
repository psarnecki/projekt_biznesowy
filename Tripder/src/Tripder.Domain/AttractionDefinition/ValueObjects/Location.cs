using Tripder.Domain.Common;

namespace Tripder.Domain.AttractionDefinition.ValueObjects;

// Represents a location value object
public class Location : ValueObject
{
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string LocationName { get; private set; } = string.Empty;

    // Parameterless constructor for EF and serialization
    private Location()
    {
    }

    // Validates the location when it is created
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

    // Returns the fields used for equality
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
        yield return LocationName;
    }
}
