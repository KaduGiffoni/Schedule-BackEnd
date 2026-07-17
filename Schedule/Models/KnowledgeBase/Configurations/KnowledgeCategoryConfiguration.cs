using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeCategoryConfiguration : IEntityTypeConfiguration<KnowledgeCategory>
{
    public void Configure(EntityTypeBuilder<KnowledgeCategory> builder)
    {
        builder.ToTable("KnowledgeCategories");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Slug).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Description).HasMaxLength(300);

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.HasOne(x => x.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}