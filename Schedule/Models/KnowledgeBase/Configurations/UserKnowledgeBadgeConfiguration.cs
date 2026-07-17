using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

public class UserKnowledgeBadgeConfiguration : IEntityTypeConfiguration<UserKnowledgeBadge>
{
    public void Configure(EntityTypeBuilder<UserKnowledgeBadge> builder)
    {
        builder.ToTable("UserKnowledgeBadges");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EarnedAt).IsRequired();
        builder.Property(x => x.LastUpdatedAt).IsRequired();
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.BadgeId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.BadgeId }).IsUnique();

        builder.HasOne(x => x.Badge)
            .WithMany(b => b.UserBadges)
            .HasForeignKey(x => x.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}