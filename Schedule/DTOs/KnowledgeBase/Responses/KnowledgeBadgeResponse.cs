using System;
using System.Collections.Generic;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    public class KnowledgeBadgeResponse
    {
        public Guid BadgeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Propriedades do estado do usuário
        public bool IsEarned { get; set; }
        public bool IsActive { get; set; }
        public DateTime? EarnedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        // Se IsActive == false (invalido/faltando algo), detalhar os artigos que faltam
        public IEnumerable<KnowledgeArticleSummaryResponse> MissingArticles { get; set; } = new List<KnowledgeArticleSummaryResponse>();

        // Estatísticas para barra de progresso
        public int TotalArticles { get; set; }
        public int ReadArticles { get; set; }
    }
}
