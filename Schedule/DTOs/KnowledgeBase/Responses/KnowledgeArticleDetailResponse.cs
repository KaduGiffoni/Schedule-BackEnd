using System;
using System.Collections.Generic;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a leitura completa do artigo.
    /// Contém o conteúdo pesado e todas as relações (Tags e Pré-requisitos).
    /// </summary>
    public record KnowledgeArticleDetailResponse
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string Summary { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public DifficultyLevel Difficulty { get; init; }
        public int EstimatedTimeInMinutes { get; init; }
        public ArticleStatus Status { get; init; }
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
        public string AuthorName { get; init; } = string.Empty;
        public int ViewCount { get; init; }
        public int FavoriteCount { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime LastUpdatedAt { get; init; }
        public IEnumerable<KnowledgeTagResponse> Tags { get; init; } = [];
        public IEnumerable<KnowledgeArticleSummaryResponse> References { get; init; } = [];
    }
}