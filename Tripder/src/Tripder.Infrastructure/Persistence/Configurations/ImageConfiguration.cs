using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        // obrazek jest dzieckiem scenariusza — jak usuniesz scenariusz to obrazki też powinny zginąć (Cascade)
        builder.ToTable("Images");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Url).HasMaxLength(2000).IsRequired();
        builder.Property(i => i.OrderIndex).IsRequired();

        builder.HasOne<Scenario>()
            .WithMany("_images")
            .HasForeignKey(i => i.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
