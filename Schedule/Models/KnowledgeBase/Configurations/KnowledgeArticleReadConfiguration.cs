using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeArticleReadConfiguration : IEntityTypeConfiguration<KnowledgeArticleRead>
{
    public void Configure(EntityTypeBuilder<KnowledgeArticleRead> builder)
    {
        builder.ToTable("KnowledgeArticleReads");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReadAt).IsRequired();
        builder.Property(x => x.ArticleId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();

        builder.HasIndex(x => new { x.ArticleId, x.UserId }).IsUnique();

        builder.HasOne(x => x.Article)
            .WithMany(a => a.ReadReceipts)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}