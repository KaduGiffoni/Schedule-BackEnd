using System;
using System.Collections.Generic;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a resposta de leitura de Categorias.
    /// Estruturado para suportar tanto listagens planas quanto hierárquicas (árvore).
    /// </summary>
    public record KnowledgeCategoryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string? Description { get; init; }
        public Guid? ParentCategoryId { get; init; }
        public IEnumerable<KnowledgeCategoryResponse> SubCategories { get; init; } = [];
    }
}