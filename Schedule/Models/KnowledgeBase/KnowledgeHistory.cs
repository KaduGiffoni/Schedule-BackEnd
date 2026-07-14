using System;
using Schedule.Models.Core;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade de Auditoria (Audit Log) para rastrear eventos administrativos em um artigo.
    /// Atende à regra RB020 (Rastreabilidade de alterações) sem poluir o histórico de texto.
    /// </summary>
    public class KnowledgeHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Ação realizada (ex: "Created", "StatusChanged", "SoftDeleted", "Restored", "NewVersion").
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Detalhes adicionais sobre a ação (ex: "Status alterado de Draft para Published").
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Data e hora exatas em que a ação ocorreu.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID do artigo em que a ação foi realizada.
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// ID do usuário (Analista/Admin) que realizou a ação.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}