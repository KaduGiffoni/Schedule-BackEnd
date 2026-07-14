using System;
using Schedule.Models.Core;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que regista cada visualização feita num artigo.
    /// Garante o cumprimento da regra RB015 (Histórico de visualizações) e serve de base para o contador (RB024).
    /// </summary>
    public class KnowledgeView
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Data e hora exatas em que o utilizador acedeu ao artigo.
        /// </summary>
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID do artigo raiz que foi acedido.
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// ID do utilizador que visualizou o artigo. 
        /// Como a regra RB001 exige autenticação para aceder à Base de Conhecimento, 
        /// este campo será sempre preenchido com o ID do analista.
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}