using System;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de criação de uma nova Categoria.
    /// Isola a entidade de domínio e garante que apenas dados permitidos sejam recebidos da API.
    /// </summary>
    public class CreateKnowledgeCategoryRequest
    {
        /// <summary>
        /// Nome de exibição da categoria. Exemplo: "Infraestrutura" ou "Switches Cisco".
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional para detalhar o propósito desta categoria.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID da categoria pai. Deve ser preenchido caso esta requisição seja 
        /// para criar uma subcategoria, atendendo à regra de negócio RB021.
        /// </summary>
        public Guid? ParentCategoryId { get; set; }
    }
}