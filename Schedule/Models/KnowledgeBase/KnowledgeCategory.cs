using System;
using System.Collections.Generic;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que representa as categorias e subcategorias da Base de Conhecimento.
    /// </summary>
    public class KnowledgeCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Nome de exibição da categoria (ex: Redes, Telefonia, Servidores).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Identificador amigável e único para URLs (ex: redes, telefonia-cisco).
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional do que esta categoria engloba.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Chave estrangeira para a categoria pai. Se for nulo, indica que é uma categoria raiz (RB021).
        /// </summary>
        public Guid? ParentCategoryId { get; set; }

        // --- Propriedades de Navegação (Entity Framework) ---

        /// <summary>
        /// Referência para a categoria pai (Auto-relacionamento).
        /// </summary>
        public virtual KnowledgeCategory? ParentCategory { get; set; }

        /// <summary>
        /// Lista de subcategorias vinculadas a esta categoria.
        /// </summary>
        public virtual ICollection<KnowledgeCategory> SubCategories { get; set; } = new List<KnowledgeCategory>();

        /* * Nota de Arquitetura: 
         * A coleção de ICollection<KnowledgeArticle> será adicionada posteriormente (via Fluent API ou aqui),
         * após a criação da entidade principal na Fase 2 para manter a compilação limpa sem dependências circulares precoces.
         */
    }
}