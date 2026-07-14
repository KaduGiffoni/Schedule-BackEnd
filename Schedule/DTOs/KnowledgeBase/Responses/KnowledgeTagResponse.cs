using System;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a resposta de leitura de Tags.
    /// Isola a entidade do banco de dados e expõe apenas os dados visuais.
    /// </summary>
    public class KnowledgeTagResponse
    {
        /// <summary>
        /// Identificador único da palavra-chave.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome de exibição da tag (ex: "Firewall" ou "Windows 11").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Slug (URL amigável) gerado pelo backend.
        /// Pode ser usado pelo frontend para montar filtros na URL (ex: /knowledge-base?tag=windows-11).
        /// </summary>
        public string Slug { get; set; } = string.Empty;
    }
}