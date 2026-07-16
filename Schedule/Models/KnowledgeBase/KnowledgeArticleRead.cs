using System;
using Schedule.Models;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que registra a confirmação de leitura de um artigo por um usuário.
    /// Essencial para trilhas de aprendizado (Onboarding - RB032) e sistema de Gamificação (RB033, RB034).
    /// </summary>
    public class KnowledgeArticleRead
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Data e hora exatas em que o usuário marcou o procedimento como lido.
        /// Se o artigo sofrer uma alteração importante no futuro e ganhar uma nova versão, 
        /// o sistema poderá comparar esta data com o "UpdatedAt" do artigo para invalidar a leitura.
        /// </summary>
        public DateTime ReadAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID do artigo raiz que foi lido.
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// ID do analista (usuário) que confirmou a leitura.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}