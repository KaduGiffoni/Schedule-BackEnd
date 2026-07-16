using System;
using Schedule.Models;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade de junção que representa o progresso e a conquista de um selo por um utilizador.
    /// Atende às regras de desbloqueio (RB033) e de invalidação/conquista cinzenta (RB034).
    /// </summary>
    public class UserKnowledgeBadge
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Data e hora exatas em que o utilizador completou os requisitos e ganhou o selo pela primeira vez.
        /// </summary>
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última alteração de estado (ex: quando o selo voltou a ficar ativo após nova leitura).
        /// </summary>
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica se a conquista está ativa (colorida) ou inativa (cinzenta).
        /// Se um novo artigo for adicionado à categoria, este campo passa a false (RB034).
        /// </summary>
        public bool IsActive { get; set; } = true;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID da conquista/selo associado.
        /// </summary>
        public Guid BadgeId { get; set; }

        /// <summary>
        /// ID do utilizador (analista) que possui este progresso.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        public virtual KnowledgeBadge? Badge { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}