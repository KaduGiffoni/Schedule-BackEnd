using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Schedule.Models.KnowledgeBase;

namespace Schedule.Interfaces.KnowledgeBase
{
    /// <summary>
    /// Contrato de repositório para a entidade KnowledgeCategory.
    /// Isola a camada de serviço do Entity Framework Core e define operações hierárquicas (RB021).
    /// </summary>
    public interface IKnowledgeCategoryRepository
    {
        /// <summary>
        /// Obtém todas as categorias de forma plana (sem aninhamento profundo).
        /// </summary>
        Task<IEnumerable<KnowledgeCategory>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém a árvore completa de categorias (Categorias Pai e suas respectivas Subcategorias).
        /// Essencial para a montagem de menus de navegação no frontend.
        /// </summary>
        Task<IEnumerable<KnowledgeCategory>> GetTreeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém uma categoria específica pelo seu ID.
        /// </summary>
        Task<KnowledgeCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém uma categoria utilizando o seu Slug (URL amigável).
        /// </summary>
        Task<KnowledgeCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adiciona uma nova categoria ao banco de dados.
        /// </summary>
        Task<KnowledgeCategory> AddAsync(KnowledgeCategory category, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza os metadados de uma categoria existente.
        /// </summary>
        Task UpdateAsync(KnowledgeCategory category, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove uma categoria fisicamente do banco de dados (Hard Delete).
        /// O EF Core bloqueará a exclusão caso existam subcategorias ou artigos vinculados (DeleteBehavior.Restrict).
        /// </summary>
        Task DeleteAsync(KnowledgeCategory category, CancellationToken cancellationToken = default);
    }
}