using System;
using System.Collections.Generic;
using Schedule.Models;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que armazena a fotografia do conteúdo de um artigo em um determinado momento.
    /// Toda edição gera um novo registro aqui, garantindo histórico e rastreabilidade (RB004, RB005).
    /// </summary>
    public class KnowledgeArticleVersion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int VersionNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Basic;
        public int EstimatedTimeInMinutes { get; set; }
        public string? ChangeDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid ArticleId { get; set; }
        public string EditorId { get; set; } = string.Empty;

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual ApplicationUser? Editor { get; set; }

        public virtual ICollection<KnowledgeMedia> Media { get; set; } = new List<KnowledgeMedia>();
    }
}