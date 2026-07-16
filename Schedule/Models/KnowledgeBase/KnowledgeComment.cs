using System;
using Schedule.Models;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que representa um comentário ou discussão em um artigo da Base de Conhecimento.
    /// Atende à regra RB016 (Comentários não alteram o artigo).
    /// </summary>
    public class KnowledgeComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Conteúdo textual do comentário feito pelo analista.
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora exatas em que o comentário foi publicado.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID do artigo principal (raiz) em que o comentário foi feito.
        /// Os comentários persistem independentemente da versão ativa do artigo.
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// ID do usuário logado que escreveu o comentário.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}