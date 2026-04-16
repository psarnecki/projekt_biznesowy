using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Configurations;

public class DayOfWeekEntryConfiguration : IEntityTypeConfiguration<DayOfWeekEntry>
{
    public void Configure(EntityTypeBuilder<DayOfWeekEntry> builder)
    {
        builder.ToTable("RuleDays");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).HasMaxLength(32).IsRequired();

        builder.HasOne<RuleDefinition>()
            .WithMany(r => r.Days)
            .HasForeignKey("RuleDefinitionId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
