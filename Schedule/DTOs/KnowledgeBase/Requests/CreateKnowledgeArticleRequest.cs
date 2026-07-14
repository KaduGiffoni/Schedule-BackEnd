using System;
using System.Collections.Generic;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de criação de um novo Artigo/Procedimento.
    /// Encapsula dados do artigo raiz e da sua primeira versão.
    /// </summary>
    public class CreateKnowledgeArticleRequest
    {
        /// <summary>
        /// Título do artigo. Regra RB008.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Resumo do artigo. Regra RB009.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo detalhado do procedimento (Markdown/HTML).
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Tempo estimado de leitura/execução em minutos. Regra RB023.
        /// </summary>
        public int EstimatedTimeInMinutes { get; set; }

        /// <summary>
        /// Nível de complexidade do procedimento. Regra RB023.
        /// </summary>
        public DifficultyLevel Difficulty { get; set; }

        /// <summary>
        /// ID da Categoria à qual este artigo pertence. Regra RB010.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Status inicial do artigo (Draft ou Published). Regra RB012.
        /// </summary>
        public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

        /// <summary>
        /// Lista de IDs das Tags associadas. Regra RB011 exige pelo menos uma.
        /// </summary>
        public IEnumerable<Guid> TagIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Lista opcional de IDs de artigos referenciados (pré-requisitos). Regra RB031.
        /// </summary>
        public IEnumerable<Guid> ReferencedArticleIds { get; set; } = new List<Guid>();
    }
}