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

        // jeden scenariusz należy do jednej atrakcji — jak masz AttractionId w tabeli to EF musi to wiedzieć żeby nie zrobić Ci losowych joinów
        builder.HasOne(s => s.Attraction)
            .WithMany("_scenarios")
            .HasForeignKey(s => s.AttractionId)
            .OnDelete(DeleteBehavior.Cascade);

        // obrazki są w osobnym pliku konfiguracji (ImageConfiguration) żeby nie duplikować tej samej relacji w dwóch miejscach (DRY)

        builder.HasMany(typeof(Tag), "_tags")
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("ScenarioTags");
            });

        builder.HasMany(typeof(RuleDefinition), "_rules")
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("ScenarioRules");
            });
    }
}
