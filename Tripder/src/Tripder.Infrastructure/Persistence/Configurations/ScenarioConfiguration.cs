using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Configurations;

public class ScenarioConfiguration : IEntityTypeConfiguration<Scenario>
{
    public void Configure(EntityTypeBuilder<Scenario> builder)
    {
        builder.ToTable("Scenarios");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Description).HasColumnType("text");
        builder.Property(s => s.DurationMinutes).IsRequired();
        builder.Property(s => s.State)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.HasOne(s => s.Attraction)
            .WithMany(a => a.Scenarios)
            .HasForeignKey(s => s.AttractionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(s => s.Tags)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("ScenarioTags");
            });

        builder.HasMany(s => s.Rules)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("ScenarioRules");
            });
    }
}
