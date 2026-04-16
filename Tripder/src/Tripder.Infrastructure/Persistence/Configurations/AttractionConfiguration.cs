using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Configurations;

public class AttractionConfiguration : IEntityTypeConfiguration<Attraction>
{
    public void Configure(EntityTypeBuilder<Attraction> builder)
    {
        builder.ToTable("Attractions");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.State)
            .HasConversion<string>()
            .HasMaxLength(32);

        builder.OwnsOne(a => a.Location, loc =>
        {
            loc.Property(l => l.Latitude).HasColumnName("Latitude");
            loc.Property(l => l.Longitude).HasColumnName("Longitude");
            loc.Property(l => l.LocationName).HasMaxLength(300).HasColumnName("LocationName");
        });

        builder.HasOne(a => a.Category)
            .WithMany()
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Tags)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("AttractionTags");
            });

        builder.HasMany(a => a.Rules)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("AttractionRules");
            });
    }
}
