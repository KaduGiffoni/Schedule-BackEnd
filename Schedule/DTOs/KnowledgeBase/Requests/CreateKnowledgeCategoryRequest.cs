using System;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de criação de uma nova Categoria.
    /// Isola a entidade de domínio e garante que apenas dados permitidos sejam recebidos da API.
    /// </summary>
    public record CreateKnowledgeCategoryRequest
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public Guid? ParentCategoryId { get; init; }
    }
}