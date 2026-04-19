using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tripder.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        var fromEnv = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return fromEnv;
        }

        var currentDir = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            Path.Combine(currentDir, "appsettings.json"),
            Path.Combine(currentDir, "..", "Tripder.Api", "appsettings.json"),
            Path.Combine(currentDir, "..", "..", "Tripder.Api", "appsettings.json")
        };

        foreach (var filePath in candidates)
        {
            if (!File.Exists(filePath))
            {
                continue;
            }

            var value = TryReadConnectionString(filePath);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        throw new InvalidOperationException(
            "Nie znaleziono connection stringa 'DefaultConnection'. Ustaw zmienną środowiskową ConnectionStrings__DefaultConnection lub dodaj ją do appsettings.json w Tripder.Api.");
    }

    private static string? TryReadConnectionString(string filePath)
    {
        using var document = JsonDocument.Parse(File.ReadAllText(filePath));

        if (!document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings))
        {
            return null;
        }

        if (!connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection))
        {
            return null;
        }

        return defaultConnection.GetString();
    }
}

    