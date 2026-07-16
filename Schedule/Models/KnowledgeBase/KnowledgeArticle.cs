using System;
using System.Collections.Generic;
using Schedule.Models;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Models.KnowledgeBase
{
    public class KnowledgeArticle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Slug { get; set; } = string.Empty;
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
        public bool IsDeleted { get; set; } = false;
        public int ViewCount { get; set; } = 0;
        public int FavoriteCount { get; set; } = 0;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public string AuthorId { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid? CurrentVersionId { get; set; }

        public virtual ApplicationUser? Author { get; set; }
        public virtual KnowledgeCategory? Category { get; set; }

        public virtual KnowledgeArticleVersion? CurrentVersion { get; set; }
        public virtual ICollection<KnowledgeArticleVersion> Versions { get; set; } = new List<KnowledgeArticleVersion>();

        public virtual ICollection<KnowledgeArticleTag> ArticleTags { get; set; } = new List<KnowledgeArticleTag>();
        public virtual ICollection<KnowledgeView> HistoryViews { get; set; } = new List<KnowledgeView>();
        public virtual ICollection<KnowledgeFavorite> Favorites { get; set; } = new List<KnowledgeFavorite>();
        public virtual ICollection<KnowledgeComment> Comments { get; set; } = new List<KnowledgeComment>();

        // Refatoração: Adicionado o lado reverso do auto-relacionamento
        public virtual ICollection<KnowledgeArticleReference> References { get; set; } = new List<KnowledgeArticleReference>();
        public virtual ICollection<KnowledgeArticleReference> ReferencedBy { get; set; } = new List<KnowledgeArticleReference>();

        public virtual ICollection<KnowledgeArticleRead> ReadReceipts { get; set; } = new List<KnowledgeArticleRead>();
    }
}