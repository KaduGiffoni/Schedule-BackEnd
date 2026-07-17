using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class KnowledgeCommentConfiguration : IEntityTypeConfiguration<KnowledgeComment>
{
    public void Configure(EntityTypeBuilder<KnowledgeComment> builder)
    {
        builder.ToTable("KnowledgeComments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content).IsRequired().HasMaxLength(1500);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.ArticleId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();

        builder.HasIndex(x => new { x.ArticleId, x.CreatedAt });

        builder.HasOne(x => x.Article)
            .WithMany(a => a.Comments)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}