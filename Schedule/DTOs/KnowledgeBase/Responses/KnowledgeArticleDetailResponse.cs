using System;
using System.Collections.Generic;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a leitura completa do artigo.
    /// Contém o conteúdo pesado e todas as relações (Tags e Pré-requisitos).
    /// </summary>
    public class KnowledgeArticleDetailResponse
    {
        /// <summary>
        /// ID do artigo raiz.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título completo do procedimento.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Slug (URL amigável) para navegação no frontend.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Resumo do que o artigo aborda.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo detalhado da versão atual (Markdown ou HTML).
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Nível de dificuldade da operação.
        /// </summary>
        public DifficultyLevel Difficulty { get; set; }

        /// <summary>
        /// Tempo estimado de execução/leitura em minutos.
        /// </summary>
        public int EstimatedTimeInMinutes { get; set; }

        /// <summary>
        /// Status atual de visibilidade.
        /// </summary>
        public ArticleStatus Status { get; set; }

        /// <summary>
        /// ID da Categoria.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Nome da Categoria.
        /// </summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Nome do criador original do procedimento.
        /// </summary>
        public string AuthorName { get; set; } = string.Empty;

        /// <summary>
        /// Total de visualizações acumuladas.
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// Total de vezes que o artigo foi favoritado.
        /// </summary>
        public int FavoriteCount { get; set; }

        /// <summary>
        /// Data em que o artigo foi criado originalmente.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data da última versão/atualização do artigo.
        /// </summary>
        public DateTime LastUpdatedAt { get; set; }

        /// <summary>
        /// Lista das tags (palavras-chave) associadas ao artigo.
        /// Retorna um DTO simples contendo o Id e o Name da tag.
        /// </summary>
        public IEnumerable<KnowledgeTagResponse> Tags { get; set; } = new List<KnowledgeTagResponse>();

        /// <summary>
        /// Lista de artigos referenciados (pré-requisitos).
        /// Reutiliza o DTO de resumo para renderizar cartões ricos no frontend.
        /// </summary>
        public IEnumerable<KnowledgeArticleSummaryResponse> References { get; set; } = new List<KnowledgeArticleSummaryResponse>();
    }
}