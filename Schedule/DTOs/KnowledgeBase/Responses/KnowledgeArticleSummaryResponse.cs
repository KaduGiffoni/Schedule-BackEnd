using System;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para listagens e resultados de pesquisa.
    /// Versão leve do artigo, sem o conteúdo completo, focada em performance.
    /// </summary>
    public class KnowledgeArticleSummaryResponse
    {
        /// <summary>
        /// ID do artigo raiz.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título do procedimento operacional.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Slug (URL amigável) para navegação no frontend.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Resumo rápido do que o artigo aborda.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Nível de dificuldade da operação (Básico, Intermediário, Avançado).
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
        /// ID da Categoria para navegação.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Nome da Categoria (achatado para facilitar a exibição no UI).
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
    }
}