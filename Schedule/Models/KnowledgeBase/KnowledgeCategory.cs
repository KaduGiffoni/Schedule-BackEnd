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

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? ParentCategoryId { get; set; }

        public virtual KnowledgeCategory? ParentCategory { get; set; }
        public virtual ICollection<KnowledgeCategory> SubCategories { get; set; } = new List<KnowledgeCategory>();

        // Refatoração: Propriedade de navegação ativada para obter os artigos da categoria.
        public virtual ICollection<KnowledgeArticle> Articles { get; set; } = new List<KnowledgeArticle>();

        public virtual ICollection<KnowledgeBadge> Badges { get; set; } = new List<KnowledgeBadge>();
    }
}