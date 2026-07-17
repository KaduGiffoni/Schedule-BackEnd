using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeArticleConfiguration : IEntityTypeConfiguration<KnowledgeArticle>
{
    public void Configure(EntityTypeBuilder<KnowledgeArticle> builder)
    {
        builder.ToTable("KnowledgeArticles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Slug).IsRequired().HasMaxLength(180);
        builder.Property(x => x.Status).IsRequired().HasConversion<int>();
        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.ViewCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.FavoriteCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.AuthorId).IsRequired();

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.IsDeleted);
        builder.HasIndex(x => x.Status);

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CurrentVersion)
            .WithMany()
            .HasForeignKey(x => x.CurrentVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}