using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Schedule.Models.KnowledgeBase;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Interfaces.KnowledgeBase
{
    /// <summary>
    /// Contrato de repositório para a entidade principal da Base de Conhecimento.
    /// Isola a camada de serviço do Entity Framework Core.
    /// </summary>
    public interface IKnowledgeArticleRepository
    {
        /// <summary>
        /// Obtém um artigo completo (com sua versão atual, autor e categoria) pelo ID.
        /// </summary>
        Task<KnowledgeArticle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém um artigo utilizando o seu Slug (URL amigável). Ideal para rotas do frontend.
        /// </summary>
        Task<KnowledgeArticle?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// Realiza a busca pública e administrativa de artigos utilizando paginação.
        /// Suportará Full-Text Search (FTS) na implementação para consultas eficientes de texto.
        /// </summary>
        /// <returns>Uma tupla contendo a lista paginada e o total de registros encontrados.</returns>
        Task<(IEnumerable<KnowledgeArticle> Articles, int TotalCount)> SearchAsync(
            string? searchTerm,
            Guid? categoryId,
            IEnumerable<Guid>? tagIds,
            ArticleStatus? status, // Nulo para admins verem tudo, 'Published' para usuários comuns
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Adiciona um novo artigo à base de dados.
        /// </summary>
        Task<KnowledgeArticle> AddAsync(KnowledgeArticle article, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza os metadados de um artigo existente.
        /// </summary>
        Task UpdateAsync(KnowledgeArticle article, CancellationToken cancellationToken = default);

        /// <summary>
        /// Realiza a exclusão lógica (Soft Delete) do artigo, atendendo à regra RB006.
        /// </summary>
        Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}