using System;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Tabela de junção explícita para o relacionamento Muitos-para-Muitos entre Artigos e Tags.
    /// Facilita a modelagem relacional no Entity Framework Core e a pesquisa de artigos relacionados (RB030).
    /// </summary>
    public class KnowledgeArticleTag
    {
        /// <summary>
        /// Chave estrangeira que aponta para o Artigo raiz.
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// Chave estrangeira que aponta para a Tag.
        /// </summary>
        public Guid TagId { get; set; }

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual KnowledgeTag? Tag { get; set; }
    }
}