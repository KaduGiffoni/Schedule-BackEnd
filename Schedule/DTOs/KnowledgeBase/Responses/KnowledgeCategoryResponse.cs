using System;
using System.Collections.Generic;

namespace Schedule.DTOs.KnowledgeBase.Responses
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a resposta de leitura de Categorias.
    /// Estruturado para suportar tanto listagens planas quanto hierárquicas (árvore).
    /// </summary>
    public class KnowledgeCategoryResponse
    {
        /// <summary>
        /// Identificador único da categoria.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nome de exibição da categoria.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Slug (URL amigável) gerado pelo backend para uso nas rotas do frontend.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional da categoria.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ID da categoria pai, preenchido caso esta seja uma subcategoria.
        /// </summary>
        public Guid? ParentCategoryId { get; set; }

        /// <summary>
        /// Lista de subcategorias aninhadas. 
        /// Essencial para a montagem de menus em árvore no frontend (RB021).
        /// </summary>
        public IEnumerable<KnowledgeCategoryResponse> SubCategories { get; set; } = new List<KnowledgeCategoryResponse>();
    }
}