using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeArticleReferenceConfiguration : IEntityTypeConfiguration<KnowledgeArticleReference>
{
    public void Configure(EntityTypeBuilder<KnowledgeArticleReference> builder)
    {
        builder.ToTable("KnowledgeArticleReferences");
        builder.HasKey(x => new { x.ArticleId, x.ReferencedArticleId });

        builder.HasOne(x => x.Article)
            .WithMany(a => a.References)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReferencedArticle)
            .WithMany()
            .HasForeignKey(x => x.ReferencedArticleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}