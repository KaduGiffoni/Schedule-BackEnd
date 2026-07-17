using System;
using System.Collections.Generic;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de atualização de um Artigo.
    /// Os dados fornecidos aqui gerarão uma nova versão do documento (RB004).
    /// </summary>
    public record UpdateKnowledgeArticleRequest
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Summary { get; init; } = string.Empty;
        public string Content { get; init; } = string.Empty;
        public int EstimatedTimeInMinutes { get; init; }
        public DifficultyLevel Difficulty { get; init; }
        public Guid CategoryId { get; init; }
        public ArticleStatus Status { get; init; }
        public IEnumerable<Guid> TagIds { get; init; } = [];
        public IEnumerable<Guid> ReferencedArticleIds { get; init; } = [];
        public string ChangeDescription { get; init; } = string.Empty;
    }
}