using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Configurations;

public class RuleDefinitionConfiguration : IEntityTypeConfiguration<RuleDefinition>
{
    public void Configure(EntityTypeBuilder<RuleDefinition> builder)
    {
        builder.ToTable("RuleDefinitions");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RuleType)
            .HasConversion<string>()
            .HasMaxLength(32);
        builder.Property(r => r.Effect)
            .HasConversion<string>()
            .HasMaxLength(32);
        builder.Property(r => r.Priority).IsRequired();
        builder.Property(r => r.Params).HasColumnType("text");
    }
}
