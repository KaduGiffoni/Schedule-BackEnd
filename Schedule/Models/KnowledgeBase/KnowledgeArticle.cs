using System;
using System.Collections.Generic;
using Schedule.Models.Core;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade principal (Aggregate Root) que representa um Artigo/Procedimento na Base de Conhecimento.
    /// Contém os metadados fixos, contadores e o relacionamento com a versão ativa.
    /// </summary>
    public class KnowledgeArticle
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Identificador amigável e único para URLs (ex: reset-de-senha-cisco). (RB007)
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Status atual do artigo (Draft, Published, Archived). (RB012)
        /// </summary>
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

        /// <summary>
        /// Flag para Soft Delete. O artigo nunca é apagado fisicamente. (RB006)
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Contador total de visualizações do artigo. (RB024)
        /// </summary>
        public int ViewCount { get; set; } = 0;

        /// <summary>
        /// Contador total de vezes que o artigo foi favoritado. (RB025)
        /// </summary>
        public int FavoriteCount { get; set; } = 0;

        /// <summary>
        /// Data de criação do artigo. (RB023)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização de qualquer metadado ou versão do artigo. (RB023)
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID do autor original que criou o artigo. Dado imutável.
        /// </summary>
        public string AuthorId { get; set; } = string.Empty;

        /// <summary>
        /// Categoria obrigatória à qual o artigo pertence. (RB010)
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Ponteiro de otimização: Indica qual é a versão (KnowledgeArticleVersion) ativa/publicada no momento.
        /// Evita JOINs complexos ao listar artigos.
        /// </summary>
        public Guid? CurrentVersionId { get; set; }

        // --- Propriedades de Navegação ---

        public virtual ApplicationUser? Author { get; set; }
        public virtual KnowledgeCategory? Category { get; set; }

        /// <summary>
        /// Histórico completo de versões de texto/conteúdo deste artigo. (RB004, RB005)
        /// </summary>
        public virtual ICollection<KnowledgeArticleVersion> Versions { get; set; } = new List<KnowledgeArticleVersion>();

        /* * As coleções abaixo serão habilitadas conforme criarmos as respectivas entidades.
         * Deixei comentado para mantermos o projeto compilando perfeitamente a cada passo.
         */

        // public virtual ICollection<KnowledgeArticleTag> ArticleTags { get; set; } = new List<KnowledgeArticleTag>();
        // public virtual ICollection<KnowledgeView> HistoryViews { get; set; } = new List<KnowledgeView>();
        // public virtual ICollection<KnowledgeFavorite> Favorites { get; set; } = new List<KnowledgeFavorite>();
        // public virtual ICollection<KnowledgeComment> Comments { get; set; } = new List<KnowledgeComment>();

        // Referências para outros artigos (RB031 - Auto-relacionamento N:N)
        // public virtual ICollection<KnowledgeArticleReference> References { get; set; } = new List<KnowledgeArticleReference>();
    }
}