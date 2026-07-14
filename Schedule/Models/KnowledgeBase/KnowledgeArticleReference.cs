using System;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade de junção para auto-relacionamento de artigos (Muitos-para-Muitos).
    /// Permite que um artigo referencie outros artigos como pré-requisitos ou leitura recomendada (RB031).
    /// </summary>
    public class KnowledgeArticleReference
    {
        /// <summary>
        /// Chave estrangeira do artigo principal (quem está fazendo a citação).
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// Chave estrangeira do artigo que está sendo referenciado (o alvo da citação).
        /// </summary>
        public Guid ReferencedArticleId { get; set; }

        // --- Propriedades de Navegação ---

        /// <summary>
        /// O artigo de origem.
        /// </summary>
        public virtual KnowledgeArticle? Article { get; set; }

        /// <summary>
        /// O artigo de destino (referenciado).
        /// </summary>
        public virtual KnowledgeArticle? ReferencedArticle { get; set; }
    }
}