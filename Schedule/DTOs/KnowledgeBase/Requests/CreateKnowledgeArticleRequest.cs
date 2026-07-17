using System;
using System.Collections.Generic;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de criação de um novo Artigo/Procedimento.
    /// Encapsula dados do artigo raiz e da sua primeira versão.
    /// </summary>
    public record CreateKnowledgeArticleRequest
    {
        public string Title { get; init; } = string.Empty;
        public string Summary { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public int EstimatedTimeInMinutes { get; init; }
        public DifficultyLevel Difficulty { get; init; }
        public Guid CategoryId { get; init; }
        public ArticleStatus Status { get; init; } = ArticleStatus.Draft;
        public IEnumerable<Guid> TagIds { get; init; } = [];
        public IEnumerable<Guid> ReferencedArticleIds { get; init; } = [];
    }
}