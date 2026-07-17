using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeArticleVersionConfiguration : IEntityTypeConfiguration<KnowledgeArticleVersion>
{
    public void Configure(EntityTypeBuilder<KnowledgeArticleVersion> builder)
    {
        builder.ToTable("KnowledgeArticleVersions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.VersionNumber).IsRequired();
        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Summary).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.Difficulty).IsRequired().HasConversion<int>();
        builder.Property(x => x.EstimatedTimeInMinutes).IsRequired();
        builder.Property(x => x.ChangeDescription).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.EditorId).IsRequired();

        builder.HasIndex(x => x.ArticleId);

        builder.HasOne(x => x.Article)
            .WithMany(a => a.Versions)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Editor)
            .WithMany()
            .HasForeignKey(x => x.EditorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}