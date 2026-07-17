using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeBadgeConfiguration : IEntityTypeConfiguration<KnowledgeBadge>
{
    public void Configure(EntityTypeBuilder<KnowledgeBadge> builder)
    {
        builder.ToTable("KnowledgeBadges");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(2048);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();

        builder.HasIndex(x => x.CategoryId);

        builder.HasOne(x => x.Category)
            .WithMany(c => c.Badges)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}