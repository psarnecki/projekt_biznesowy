using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tripder.Domain.AttractionDefinition.Repositories;
using Tripder.Domain.Common;
using Tripder.Infrastructure.Persistence;
using Tripder.Infrastructure.Persistence.Repositories;

namespace Tripder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // connection string z appsettings — jak go nie ma to lecimy wyjątkiem od razu, bo i tak nie ma sensu odpalać apki bez bazy
        var cs = configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Brak ConnectionStrings:DefaultConnection w konfiguracji.");

        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(cs));

        services.AddScoped<IAttractionRepository, AttractionRepository>();
        services.AddScoped<IRuleDefinitionRepository, RuleDefinitionRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
