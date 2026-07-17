using System;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a resposta de leitura de Tags.
    /// Isola a entidade do banco de dados e expõe apenas os dados visuais.
    /// </summary>
    public record KnowledgeTagResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
    }
}