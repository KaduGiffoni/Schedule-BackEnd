using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Schedule.Models.KnowledgeBase;

namespace Schedule.Interfaces.KnowledgeBase
{
    /// <summary>
    /// Contrato de repositório para a entidade KnowledgeTag.
    /// Gerencia as palavras-chave usadas para busca e relacionamento de artigos.
    /// </summary>
    public interface IKnowledgeTagRepository
    {
        /// <summary>
        /// Obtém todas as tags cadastradas no sistema.
        /// </summary>
        Task<IEnumerable<KnowledgeTag>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém uma tag específica pelo seu ID.
        /// </summary>
        Task<KnowledgeTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém uma tag específica pelo seu Slug (URL/Identificador amigável).
        /// Essencial para validar a unicidade antes da inserção (RB022).
        /// </summary>
        Task<KnowledgeTag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adiciona uma nova tag ao banco de dados.
        /// </summary>
        Task<KnowledgeTag> AddAsync(KnowledgeTag tag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza os dados de uma tag existente.
        /// </summary>
        Task UpdateAsync(KnowledgeTag tag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove uma tag fisicamente do banco de dados (Hard Delete).
        /// A exclusão em cascata limpará automaticamente as referências nos artigos.
        /// </summary>
        Task DeleteAsync(KnowledgeTag tag, CancellationToken cancellationToken = default);
    }
}