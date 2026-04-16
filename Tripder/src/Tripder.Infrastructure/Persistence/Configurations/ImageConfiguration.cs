using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripder.Domain.AttractionDefinition.Entities;

namespace Tripder.Infrastructure.Persistence.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("Images");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Url).HasMaxLength(2000).IsRequired();
        builder.Property(i => i.OrderIndex).IsRequired();

        builder.HasOne<Scenario>()
            .WithMany(s => s.Images)
            .HasForeignKey(i => i.ScenarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
