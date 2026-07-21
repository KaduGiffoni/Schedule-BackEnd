using System;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    public class KnowledgeArticleHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public int Version { get; set; }
        public string ChangeDescription { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string EditorName { get; set; } = string.Empty;
    }
}
