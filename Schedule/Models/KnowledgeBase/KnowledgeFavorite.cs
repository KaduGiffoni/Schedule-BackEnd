using System;
using Schedule.Models.Core;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que registra quando um usuário favorita (salva) um artigo.
    /// Atende à regra RB014 (Favoritos pertencem ao usuário).
    /// </summary>
    public class KnowledgeFavorite
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Data e hora exatas em que o usuário favoritou o artigo.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID do artigo que foi favoritado.
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// ID do usuário que favoritou o artigo.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}