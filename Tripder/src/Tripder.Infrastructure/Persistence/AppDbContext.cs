using Microsoft.EntityFrameworkCore;
using Tripder.Domain.AttractionDefinition.Entities;
using Tripder.Domain.Common;

namespace Tripder.Infrastructure.Persistence;

// DbContext = "okno" do bazy — tutaj EF trzyma tracking encji itd. (Jak ktoś pyta: tak, to jest ten sam DbContext co na wykładzie)
public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Attraction> Attractions => Set<Attraction>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Scenario> Scenarios => Set<Scenario>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Image> Images => Set<Image>();
    public DbSet<RuleDefinition> RuleDefinitions => Set<RuleDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken) =>
        SaveChangesAsync(cancellationToken);
}
