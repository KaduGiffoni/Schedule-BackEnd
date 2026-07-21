using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Schedule.DTOs.KnowledgeBase.Requests;
using Schedule.DTOs.KnowledgeBase.Responses;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Interfaces.KnowledgeBase
{
    /// <summary>
    /// Contrato principal da camada de Serviços da Base de Conhecimento.
    /// Orquestra repositórios, validações de negócio e garante que os Controllers permaneçam finos.
    /// </summary>
    public interface IKnowledgeBaseService
    {
        // ==========================================
        // --- GESTÃO DE CATEGORIAS ---
        // ==========================================

        Task<IEnumerable<KnowledgeCategoryResponse>> GetCategoryTreeAsync(CancellationToken cancellationToken = default);
        Task<KnowledgeCategoryResponse> CreateCategoryAsync(CreateKnowledgeCategoryRequest request, CancellationToken cancellationToken = default);
        Task<KnowledgeCategoryResponse> UpdateCategoryAsync(UpdateKnowledgeCategoryRequest request, CancellationToken cancellationToken = default);
        Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);

        // ==========================================
        // --- GESTÃO DE ARTIGOS / PROCEDIMENTOS ---
        // ==========================================

        Task<(IEnumerable<KnowledgeArticleSummaryResponse> Articles, int TotalCount)> SearchArticlesAsync(
            string? searchTerm,
            Guid? categoryId,
            IEnumerable<Guid>? tagIds,
            ArticleStatus? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<KnowledgeArticleDetailResponse> GetArticleByIdAsync(Guid id, string? userId = null, CancellationToken cancellationToken = default);
        Task<KnowledgeArticleDetailResponse> GetArticleBySlugAsync(string slug, string? userId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<KnowledgeArticleHistoryResponse>> GetArticleHistoryAsync(Guid articleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cria o artigo raiz e a sua primeira versão de conteúdo.
        /// </summary>
        Task<KnowledgeArticleDetailResponse> CreateArticleAsync(CreateKnowledgeArticleRequest request, string authorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gera uma nova versão do artigo, mantendo o histórico de edições (RB004).
        /// </summary>
        Task<KnowledgeArticleDetailResponse> UpdateArticleAsync(UpdateKnowledgeArticleRequest request, string editorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executa o Soft Delete do artigo (RB006).
        /// </summary>
        Task SoftDeleteArticleAsync(Guid id, string userId, CancellationToken cancellationToken = default);

        // ==========================================
        // --- INTERAÇÕES E GAMIFICAÇÃO ---
        // ==========================================

        /// <summary>
        /// Regista a visualização de um artigo por um utilizador (RB015, RB024).
        /// </summary>
        Task RegisterViewAsync(Guid articleId, string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adiciona ou remove um artigo da lista de favoritos do utilizador (RB014, RB025).
        /// </summary>
        Task ToggleFavoriteAsync(Guid articleId, string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marca o artigo como lido, servindo de base para o sistema de gamificação (RB032, RB034).
        /// Retorna true se uma insígnia foi recém-desbloqueada nesta ação.
        /// </summary>
        Task<bool> MarkArticleAsReadAsync(Guid articleId, string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém o progresso das conquistas do utilizador.
        /// </summary>
        Task<IEnumerable<KnowledgeBadgeResponse>> GetMyBadgesAsync(string userId, CancellationToken cancellationToken = default);

        // ==========================================
        // --- GESTÃO DE TAGS ---
        // ==========================================
        Task<IEnumerable<KnowledgeTagResponse>> GetAllTagsAsync(CancellationToken cancellationToken = default);
        Task<KnowledgeTagResponse> CreateTagAsync(CreateKnowledgeTagRequest request, CancellationToken cancellationToken = default);
        Task DeleteTagAsync(Guid id, CancellationToken cancellationToken = default);
    }
}