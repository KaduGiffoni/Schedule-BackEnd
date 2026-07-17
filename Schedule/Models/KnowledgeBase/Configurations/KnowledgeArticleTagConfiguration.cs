using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeArticleTagConfiguration : IEntityTypeConfiguration<KnowledgeArticleTag>
{
    public void Configure(EntityTypeBuilder<KnowledgeArticleTag> builder)
    {
        builder.ToTable("KnowledgeArticleTags");
        builder.HasKey(x => new { x.ArticleId, x.TagId });

        builder.HasOne(x => x.Article)
            .WithMany(a => a.ArticleTags)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Tag)
            .WithMany()
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}