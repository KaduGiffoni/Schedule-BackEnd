using System;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de atualização de uma Categoria existente.
    /// Exige o Id para garantir a identificação correta do registo no banco de dados.
    /// </summary>
    public class UpdateKnowledgeCategoryRequest
    {
        /// <summary>
        /// Identificador único da categoria a ser atualizada. (Obrigatório)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Novo nome de exibição da categoria.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Nova descrição opcional da categoria.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID da categoria pai. Permite mover a categoria na árvore hierárquica (RB021).
        /// </summary>
        public Guid? ParentCategoryId { get; set; }
    }
}