using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tripder.Application.AttractionDefinition.Repositories;
using Tripder.Domain.Common;
using Tripder.Infrastructure.Persistence;
using Tripder.Infrastructure.Persistence.Repositories;

namespace Tripder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var cs = configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Brak ConnectionStrings:DefaultConnection w konfiguracji.");

        services.AddDbContext<AppDbContext>(o =>
            o.UseNpgsql(cs));

        services.AddScoped<Tripder.Application.AttractionDefinition.Repositories.IAttractionRepository, AttractionRepository>();
        services.AddScoped<Tripder.Domain.AttractionDefinition.Repositories.IAttractionRepository, AttractionRepository>();
        
        services.AddScoped<IScenarioRepository, ScenarioRepository>();
        services.AddScoped<IRuleDefinitionRepository, RuleDefinitionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
