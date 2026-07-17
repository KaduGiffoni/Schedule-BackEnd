using System;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de atualização de uma Categoria existente.
    /// Exige o Id para garantir a identificação correta do registo no banco de dados.
    /// </summary>
    public record UpdateKnowledgeCategoryRequest
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public Guid? ParentCategoryId { get; init; }
    }
}