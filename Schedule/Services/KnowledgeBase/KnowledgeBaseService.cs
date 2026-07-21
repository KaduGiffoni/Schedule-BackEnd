using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.DTOs.KnowledgeBase.Requests;
using Schedule.DTOs.KnowledgeBase.Responses;
using Schedule.Interfaces.KnowledgeBase;
using Schedule.Models.KnowledgeBase;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Services.KnowledgeBase;

/// <summary>
/// Implementação principal da camada de Serviços da Base de Conhecimento.
/// Centraliza todas as regras de negócio de versionamento, interações e gamificação.
/// </summary>
public class KnowledgeBaseService : IKnowledgeBaseService
{
    private readonly IKnowledgeArticleRepository _articleRepo;
    private readonly IKnowledgeCategoryRepository _categoryRepo;
    private readonly IKnowledgeTagRepository _tagRepo;
    private readonly IMapper _mapper;

    // TODO (Dívida Técnica): O uso do ApplicationDbContext direto no Service viola a Clean Architecture.
    // Futuramente, extraia estas chamadas para um IInteractionRepository ou utilize o padrão UnitOfWork.
    private readonly ApplicationDbContext _context;

    // Expressões regulares compiladas estaticamente para máxima performance e zero alocação repetitiva.
    private static readonly Regex InvalidCharsRegex = new(@"[^a-z0-9\s-]", RegexOptions.Compiled);
    private static readonly Regex SpacesRegex = new(@"\s+", RegexOptions.Compiled);

    public KnowledgeBaseService(
        IKnowledgeArticleRepository articleRepo,
        IKnowledgeCategoryRepository categoryRepo,
        IKnowledgeTagRepository tagRepo,
        ApplicationDbContext context,
        IMapper mapper)
    {
        _articleRepo = articleRepo;
        _categoryRepo = categoryRepo;
        _tagRepo = tagRepo;
        _context = context;
        _mapper = mapper;
    }

    #region --- GESTÃO DE CATEGORIAS ---

    public async Task<IEnumerable<KnowledgeCategoryResponse>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepo.GetTreeAsync(cancellationToken);
        return _mapper.Map<IEnumerable<KnowledgeCategoryResponse>>(categories);
    }

    public async Task<KnowledgeCategoryResponse> CreateCategoryAsync(CreateKnowledgeCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = _mapper.Map<KnowledgeCategory>(request);
        category.Slug = GenerateSlug(request.Name);

        await _categoryRepo.AddAsync(category, cancellationToken);
        return _mapper.Map<KnowledgeCategoryResponse>(category);
    }

    public async Task<KnowledgeCategoryResponse> UpdateCategoryAsync(UpdateKnowledgeCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Categoria com Id {request.Id} não encontrada.");

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;

        // Mantemos o Slug imutável por padrão (RB007) para não quebrar SEO/links salvos pelo usuário

        await _categoryRepo.UpdateAsync(category, cancellationToken);
        return _mapper.Map<KnowledgeCategoryResponse>(category);
    }

    public async Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepo.GetByIdAsync(id, cancellationToken);
        if (category != null)
        {
            await _categoryRepo.DeleteAsync(category, cancellationToken);
        }
    }

    public async Task<IEnumerable<KnowledgeTagResponse>> GetAllTagsAsync(CancellationToken cancellationToken = default)
    {
        var tags = await _tagRepo.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<KnowledgeTagResponse>>(tags);
    }

    public async Task<KnowledgeTagResponse> CreateTagAsync(CreateKnowledgeTagRequest request, CancellationToken cancellationToken = default)
    {
        var tag = new KnowledgeTag
        {
            Name = request.Name,
            Slug = GenerateSlug(request.Name)
        };

        await _tagRepo.AddAsync(tag, cancellationToken);
        return _mapper.Map<KnowledgeTagResponse>(tag);
    }

    public async Task DeleteTagAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _tagRepo.GetByIdAsync(id, cancellationToken);
        if (tag != null)
        {
            await _tagRepo.DeleteAsync(tag, cancellationToken);
        }
    }

    #endregion

    #region --- GESTÃO DE ARTIGOS ---

    public async Task<(IEnumerable<KnowledgeArticleSummaryResponse> Articles, int TotalCount)> SearchArticlesAsync(
        string? searchTerm, Guid? categoryId, IEnumerable<Guid>? tagIds, ArticleStatus? status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (articles, totalCount) = await _articleRepo.SearchAsync(searchTerm, categoryId, tagIds, status, pageNumber, pageSize, cancellationToken);
        var mappedArticles = _mapper.Map<IEnumerable<KnowledgeArticleSummaryResponse>>(articles);
        return (mappedArticles, totalCount);
    }

    public async Task<KnowledgeArticleDetailResponse> GetArticleByIdAsync(Guid id, string? userId = null, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Artigo com Id {id} não encontrado.");

        var response = _mapper.Map<KnowledgeArticleDetailResponse>(article);
        
        if (!string.IsNullOrEmpty(userId))
        {
            response.IsRead = await _context.KnowledgeArticleReads
                .AnyAsync(r => r.ArticleId == id && r.UserId == userId, cancellationToken);

            response.IsFavorited = await _context.KnowledgeFavorites
                .AnyAsync(f => f.ArticleId == id && f.UserId == userId, cancellationToken);
        }

        return response;
    }

    public async Task<KnowledgeArticleDetailResponse> GetArticleBySlugAsync(string slug, string? userId = null, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepo.GetBySlugAsync(slug, cancellationToken)
            ?? throw new KeyNotFoundException($"Artigo com Slug '{slug}' não encontrado.");

        var response = _mapper.Map<KnowledgeArticleDetailResponse>(article);
        
        if (!string.IsNullOrEmpty(userId))
        {
            response.IsRead = await _context.KnowledgeArticleReads
                .AnyAsync(r => r.ArticleId == article.Id && r.UserId == userId, cancellationToken);

            response.IsFavorited = await _context.KnowledgeFavorites
                .AnyAsync(f => f.ArticleId == article.Id && f.UserId == userId, cancellationToken);
        }

        return response;
    }

    public async Task<IEnumerable<KnowledgeArticleHistoryResponse>> GetArticleHistoryAsync(Guid articleId, CancellationToken cancellationToken = default)
    {
        var versions = await _context.KnowledgeArticleVersions
            .Include(v => v.Editor)
            .Where(v => v.ArticleId == articleId)
            .OrderByDescending(v => v.VersionNumber)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<KnowledgeArticleHistoryResponse>>(versions);
    }

    public async Task<KnowledgeArticleDetailResponse> CreateArticleAsync(CreateKnowledgeArticleRequest request, string authorId, CancellationToken cancellationToken = default)
    {
        // 1. Cria a entidade base imutável (RB023)
        var article = new KnowledgeArticle
        {
            AuthorId = authorId,
            CategoryId = request.CategoryId,
            Status = request.Status,
            Slug = GenerateSlug(request.Title)
        };

        // 2. Cria a primeira versão do artigo (RB004)
        var initialVersion = new KnowledgeArticleVersion
        {
            Title = request.Title,
            Summary = request.Summary,
            Content = request.Content,
            EstimatedTimeInMinutes = request.EstimatedTimeInMinutes,
            Difficulty = request.Difficulty,
            EditorId = authorId,
            VersionNumber = 1,
            ChangeDescription = "Criação original do procedimento."
        };

        article.Versions.Add(initialVersion);

        // 3. Processa Tags (RB011, RB022)
        foreach (var tagId in request.TagIds)
        {
            article.ArticleTags.Add(new KnowledgeArticleTag { TagId = tagId });
        }

        // 4. Processa Referências (RB031)
        foreach (var refId in request.ReferencedArticleIds.Where(id => id != article.Id).Distinct())
        {
            article.References.Add(new KnowledgeArticleReference { ReferencedArticleId = refId });
        }

        // 5. Salva no banco e define o ponteiro da versão atual
        await _articleRepo.AddAsync(article, cancellationToken);

        article.CurrentVersionId = initialVersion.Id;
        await _articleRepo.UpdateAsync(article, cancellationToken);

        // 6. Log assíncrono isolado
        await LogHistoryAsync(article.Id, authorId, "Artigo Criado", cancellationToken);

        return await GetArticleByIdAsync(article.Id, userId: null, cancellationToken);
    }

    public async Task<KnowledgeArticleDetailResponse> UpdateArticleAsync(UpdateKnowledgeArticleRequest request, string editorId, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Artigo com Id {request.Id} não encontrado.");

        // 1. Regra de Negócio Crítica (RB004): Nova Versão em vez de Update direto no conteúdo
        var lastVersionNumber = article.CurrentVersion?.VersionNumber ?? 0;

        var newVersion = new KnowledgeArticleVersion
        {
            ArticleId = article.Id,
            Title = request.Title,
            Summary = request.Summary,
            Content = request.Content,
            EstimatedTimeInMinutes = request.EstimatedTimeInMinutes,
            Difficulty = request.Difficulty,
            EditorId = editorId,
            VersionNumber = lastVersionNumber + 1,
            ChangeDescription = string.IsNullOrWhiteSpace(request.ChangeDescription)
                                ? "Atualização de procedimento."
                                : request.ChangeDescription // RB020
        };

        _context.KnowledgeArticleVersions.Add(newVersion);

        // 2. Atualiza os metadados raiz do artigo
        article.CategoryId = request.CategoryId;
        article.Status = request.Status;

        // 3. Atualização total das Tags
        article.ArticleTags.Clear();
        foreach (var tagId in request.TagIds)
        {
            article.ArticleTags.Add(new KnowledgeArticleTag { TagId = tagId });
        }

        // 4. Atualização total das Referências
        article.References.Clear();
        foreach (var refId in request.ReferencedArticleIds.Where(id => id != article.Id).Distinct())
        {
            article.References.Add(new KnowledgeArticleReference { ReferencedArticleId = refId });
        }

        // 5. Atualiza o ponteiro de versão e salva tudo
        await _context.SaveChangesAsync(cancellationToken);
        article.CurrentVersionId = newVersion.Id;
        await _articleRepo.UpdateAsync(article, cancellationToken);

        // 6. Gamificação e Auditoria
        await InvalidateBadgesForCategoryAsync(article.CategoryId, cancellationToken);
        await LogHistoryAsync(article.Id, editorId, $"Nova versão ({newVersion.VersionNumber}) gerada.", cancellationToken);

        return await GetArticleByIdAsync(article.Id, userId: null, cancellationToken);
    }

    public async Task SoftDeleteArticleAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        await _articleRepo.SoftDeleteAsync(id, cancellationToken);
        await LogHistoryAsync(id, userId, "Artigo arquivado/excluído logicamente.", cancellationToken);
    }

    #endregion

    #region --- INTERAÇÕES E GAMIFICAÇÃO ---

    public async Task RegisterViewAsync(Guid articleId, string userId, CancellationToken cancellationToken = default)
    {
        var view = new KnowledgeView { ArticleId = articleId, UserId = userId };
        await _context.KnowledgeViews.AddAsync(view, cancellationToken);

        var article = await _context.KnowledgeArticles.FindAsync(new object[] { articleId }, cancellationToken);
        if (article != null)
        {
            article.ViewCount++;
            _context.KnowledgeArticles.Update(article);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ToggleFavoriteAsync(Guid articleId, string userId, CancellationToken cancellationToken = default)
    {
        var article = await _context.KnowledgeArticles.FindAsync(new object[] { articleId }, cancellationToken)
            ?? throw new KeyNotFoundException("Artigo não encontrado para marcação de favorito.");

        var existingFav = await _context.KnowledgeFavorites
            .FirstOrDefaultAsync(f => f.ArticleId == articleId && f.UserId == userId, cancellationToken);

        if (existingFav != null)
        {
            _context.KnowledgeFavorites.Remove(existingFav);
            article.FavoriteCount = Math.Max(0, article.FavoriteCount - 1);
        }
        else
        {
            await _context.KnowledgeFavorites.AddAsync(new KnowledgeFavorite { ArticleId = articleId, UserId = userId }, cancellationToken);
            article.FavoriteCount++;
        }

        _context.KnowledgeArticles.Update(article);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> MarkArticleAsReadAsync(Guid articleId, string userId, CancellationToken cancellationToken = default)
    {
        bool alreadyRead = await _context.KnowledgeArticleReads
            .AnyAsync(r => r.ArticleId == articleId && r.UserId == userId, cancellationToken);

        if (alreadyRead) return false;

        // Adiciona o recibo de leitura
        await _context.KnowledgeArticleReads.AddAsync(new KnowledgeArticleRead { ArticleId = articleId, UserId = userId }, cancellationToken);

        // Salvar no banco ANTES da gamificação para o Count() funcionar corretamente
        await _context.SaveChangesAsync(cancellationToken);

        // Dispara análise da gamificação
        bool badgeUnlocked = await ProcessGamificationAsync(articleId, userId, cancellationToken);

        
        return badgeUnlocked;
    }

    public async Task<IEnumerable<KnowledgeBadgeResponse>> GetMyBadgesAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Auto-gera insígnias para categorias que não possuem uma (para que não fique vazia a tela)
        var categoriesWithoutBadges = await _context.KnowledgeCategories
            .Where(c => !_context.KnowledgeBadges.Any(b => b.CategoryId == c.Id))
            .ToListAsync(cancellationToken);

        if (categoriesWithoutBadges.Any())
        {
            foreach (var cat in categoriesWithoutBadges)
            {
                _context.KnowledgeBadges.Add(new KnowledgeBadge
                {
                    Name = $"Especialista em {cat.Name}",
                    Description = $"Você leu todos os procedimentos da categoria {cat.Name}.",
                    CategoryId = cat.Id
                });
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        var allBadges = await _context.KnowledgeBadges
            .Include(b => b.Category)
            .ToListAsync(cancellationToken);

        var userBadges = await _context.UserKnowledgeBadges
            .Where(ub => ub.UserId == userId)
            .ToDictionaryAsync(ub => ub.BadgeId, cancellationToken);

        var response = new List<KnowledgeBadgeResponse>();

        foreach (var badge in allBadges)
        {
            var userBadge = userBadges.ContainsKey(badge.Id) ? userBadges[badge.Id] : null;

            var badgeDto = new KnowledgeBadgeResponse
            {
                BadgeId = badge.Id,
                Name = badge.Name,
                Description = badge.Description,
                ImageUrl = badge.ImageUrl,
                CategoryId = badge.CategoryId,
                CategoryName = badge.Category?.Name ?? string.Empty,
                IsEarned = userBadge != null,
                IsActive = userBadge?.IsActive ?? false,
                EarnedAt = userBadge?.EarnedAt,
                LastUpdatedAt = userBadge?.LastUpdatedAt
            };

            // Artigos da categoria que estão publicados e não deletados
            var categoryArticles = await _context.KnowledgeArticles
                .Include(a => a.CurrentVersion)
                .Where(a => a.CategoryId == badge.CategoryId && a.Status == ArticleStatus.Published && !a.IsDeleted)
                .ToListAsync(cancellationToken);

            // IDs lidos pelo usuário
            var readArticleIds = await _context.KnowledgeArticleReads
                .Where(r => r.UserId == userId && r.Article.CategoryId == badge.CategoryId)
                .Select(r => r.ArticleId)
                .ToListAsync(cancellationToken);

            badgeDto.TotalArticles = categoryArticles.Count;
            // Apenas considerar como "lidos" os artigos que ainda existem e estão publicados
            badgeDto.ReadArticles = categoryArticles.Count(a => readArticleIds.Contains(a.Id));

            // Se o usuário não tem a badge ou ela está inativa, detalhar o que falta ler
            if (!badgeDto.IsActive)
            {
                var missingArticles = categoryArticles
                    .Where(a => !readArticleIds.Contains(a.Id))
                    .Select(a => _mapper.Map<KnowledgeArticleSummaryResponse>(a))
                    .ToList();

                badgeDto.MissingArticles = missingArticles;
            }

            response.Add(badgeDto);
        }

        return response.OrderByDescending(b => b.IsActive).ThenByDescending(b => b.EarnedAt).ToList();
    }

    #endregion

    #region --- MÉTODOS PRIVADOS AUXILIARES ---

    private async Task LogHistoryAsync(Guid articleId, string userId, string action, CancellationToken cancellationToken)
    {
        var log = new KnowledgeHistory
        {
            ArticleId = articleId,
            UserId = userId,
            Action = action
        };
        await _context.KnowledgeHistories.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> ProcessGamificationAsync(Guid articleId, string userId, CancellationToken cancellationToken)
    {
        var article = await _context.KnowledgeArticles.FindAsync(new object[] { articleId }, cancellationToken);
        if (article == null) return false;

        var badge = await _context.KnowledgeBadges.FirstOrDefaultAsync(b => b.CategoryId == article.CategoryId, cancellationToken);
        if (badge == null)
        {
            var category = await _context.KnowledgeCategories.FindAsync(new object[] { article.CategoryId }, cancellationToken);
            if (category == null) return false;

            badge = new KnowledgeBadge
            {
                Name = $"Especialista em {category.Name}",
                Description = $"Você leu todos os procedimentos da categoria {category.Name}.",
                CategoryId = category.Id
            };
            await _context.KnowledgeBadges.AddAsync(badge, cancellationToken);
            // Salva logo para podermos referenciar o Id
            await _context.SaveChangesAsync(cancellationToken);
        }

        var totalArticlesInCategory = await _context.KnowledgeArticles
            .CountAsync(a => a.CategoryId == article.CategoryId && a.Status == ArticleStatus.Published && !a.IsDeleted, cancellationToken);

        var articlesReadByUser = await _context.KnowledgeArticleReads
            .CountAsync(r => r.UserId == userId && r.Article.CategoryId == article.CategoryId && r.Article.Status == ArticleStatus.Published && !r.Article.IsDeleted, cancellationToken);

        if (articlesReadByUser >= totalArticlesInCategory && totalArticlesInCategory > 0)
        {
            var userBadge = await _context.UserKnowledgeBadges
                .FirstOrDefaultAsync(ub => ub.BadgeId == badge.Id && ub.UserId == userId, cancellationToken);

            if (userBadge == null)
            {
                await _context.UserKnowledgeBadges.AddAsync(new UserKnowledgeBadge { BadgeId = badge.Id, UserId = userId, IsActive = true }, cancellationToken);
                return true; // Acabou de desbloquear pela primeira vez
            }
            else
            {
                bool wasInactive = !userBadge.IsActive;
                userBadge.IsActive = true;
                userBadge.LastUpdatedAt = DateTime.UtcNow;
                _context.UserKnowledgeBadges.Update(userBadge);
                return wasInactive; // Se estava inativa e ativou, retorna true para comemorar de novo
            }
        }
        
        return false;
    }

    private async Task InvalidateBadgesForCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        var badge = await _context.KnowledgeBadges.FirstOrDefaultAsync(b => b.CategoryId == categoryId, cancellationToken);
        if (badge != null)
        {
            var userBadges = await _context.UserKnowledgeBadges.Where(ub => ub.BadgeId == badge.Id && ub.IsActive).ToListAsync(cancellationToken);
            foreach (var ub in userBadges)
            {
                ub.IsActive = false;
            }
            if (userBadges.Any())
            {
                _context.UserKnowledgeBadges.UpdateRange(userBadges);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private static string GenerateSlug(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase)) return string.Empty;

        var str = phrase.ToLowerInvariant();
        str = InvalidCharsRegex.Replace(str, "");
        str = SpacesRegex.Replace(str, " ").Trim();

        if (str.Length > 60)
        {
            str = str.Substring(0, 60).Trim();
        }

        return str.Replace(" ", "-");
    }

    #endregion
}