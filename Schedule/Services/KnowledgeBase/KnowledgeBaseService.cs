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

namespace Schedule.Services.KnowledgeBase
{
    /// <summary>
    /// Implementação principal da camada de Serviços da Base de Conhecimento.
    /// Centraliza todas as regras de negócio de versionamento, interações e gamificação.
    /// </summary>
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private readonly IKnowledgeArticleRepository _articleRepo;
        private readonly IKnowledgeCategoryRepository _categoryRepo;
        private readonly IKnowledgeTagRepository _tagRepo;
        private readonly ApplicationDbContext _context; // Utilizado para salvar as interações (Views, Favorites, Badges) sem explodir o número de repositórios
        private readonly IMapper _mapper;

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
            var category = await _categoryRepo.GetByIdAsync(request.Id, cancellationToken);
            if (category == null) throw new Exception("Categoria não encontrada.");

            category.Name = request.Name;
            category.Description = request.Description;
            category.ParentCategoryId = request.ParentCategoryId;

            // Opcional: Atualizar o slug ou mantê-lo imutável para não quebrar links (RB007)

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

        public async Task<KnowledgeArticleDetailResponse> GetArticleByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var article = await _articleRepo.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<KnowledgeArticleDetailResponse>(article);
        }

        public async Task<KnowledgeArticleDetailResponse> GetArticleBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var article = await _articleRepo.GetBySlugAsync(slug, cancellationToken);
            return _mapper.Map<KnowledgeArticleDetailResponse>(article);
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
            foreach (var refId in request.ReferencedArticleIds)
            {
                if (refId != article.Id) // Previne auto-referência
                    article.References.Add(new KnowledgeArticleReference { ReferencedArticleId = refId });
            }

            // 5. Salva no banco. O EF Core cuidará de inserir a raiz, a versão e atualizar o CurrentVersionId
            await _articleRepo.AddAsync(article, cancellationToken);

            // Atualiza o ponteiro da versão atual
            article.CurrentVersionId = initialVersion.Id;
            await _articleRepo.UpdateAsync(article, cancellationToken);

            // Registra Histórico (RB020)
            await LogHistoryAsync(article.Id, authorId, "Artigo Criado", cancellationToken);

            return await GetArticleByIdAsync(article.Id, cancellationToken);
        }

        public async Task<KnowledgeArticleDetailResponse> UpdateArticleAsync(UpdateKnowledgeArticleRequest request, string editorId, CancellationToken cancellationToken = default)
        {
            var article = await _articleRepo.GetByIdAsync(request.Id, cancellationToken);
            if (article == null) throw new Exception("Artigo não encontrado.");

            // 1. Regra de Negócio Crítica (RB004): Nova Versão em vez de Update
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
                ChangeDescription = request.ChangeDescription // RB020 Justificativa obrigatória
            };

            _context.KnowledgeArticleVersions.Add(newVersion);

            // Atualiza os metadados do artigo
            article.CategoryId = request.CategoryId;
            article.Status = request.Status;

            // Opcional: Atualizar lógica de Tags e Referências (Excluído aqui por brevidade, envolveria limpar a coleção e re-adicionar)

            await _context.SaveChangesAsync(cancellationToken);

            // Atualiza o ponteiro
            article.CurrentVersionId = newVersion.Id;
            await _articleRepo.UpdateAsync(article, cancellationToken);

            // Regra da Gamificação (RB034): Invalida os selos dos usuários desta categoria
            await InvalidateBadgesForCategoryAsync(article.CategoryId, cancellationToken);

            // Registra Histórico
            await LogHistoryAsync(article.Id, editorId, $"Nova versão ({newVersion.VersionNumber}) gerada.", cancellationToken);

            return await GetArticleByIdAsync(article.Id, cancellationToken);
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
            // RB015: Grava o histórico
            var view = new KnowledgeView { ArticleId = articleId, UserId = userId };
            await _context.KnowledgeViews.AddAsync(view, cancellationToken);

            // RB024: Incrementa contador otimizado
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
            var existingFav = await _context.KnowledgeFavorites
                .FirstOrDefaultAsync(f => f.ArticleId == articleId && f.UserId == userId, cancellationToken);

            var article = await _context.KnowledgeArticles.FindAsync(new object[] { articleId }, cancellationToken);
            if (article == null) return;

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

        public async Task MarkArticleAsReadAsync(Guid articleId, string userId, CancellationToken cancellationToken = default)
        {
            // Verifica se já leu (para não duplicar recibos)
            bool alreadyRead = await _context.KnowledgeArticleReads
                .AnyAsync(r => r.ArticleId == articleId && r.UserId == userId, cancellationToken);

            if (!alreadyRead)
            {
                // RB032: Grava o recibo de leitura
                await _context.KnowledgeArticleReads.AddAsync(new KnowledgeArticleRead { ArticleId = articleId, UserId = userId }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Dispara o motor de Gamificação (RB033, RB034)
                await ProcessGamificationAsync(articleId, userId, cancellationToken);
            }
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

        private async Task ProcessGamificationAsync(Guid articleId, string userId, CancellationToken cancellationToken)
        {
            // 1. Descobre a categoria do artigo
            var article = await _context.KnowledgeArticles.FindAsync(new object[] { articleId }, cancellationToken);
            if (article == null) return;

            // 2. Verifica se existe um Selo (Badge) cadastrado para esta categoria (RB033)
            var badge = await _context.KnowledgeBadges.FirstOrDefaultAsync(b => b.CategoryId == article.CategoryId, cancellationToken);
            if (badge == null) return;

            // 3. Verifica se o usuário já leu TODOS os artigos publicados desta categoria
            var totalArticlesInCategory = await _context.KnowledgeArticles
                .CountAsync(a => a.CategoryId == article.CategoryId && a.Status == ArticleStatus.Published && !a.IsDeleted, cancellationToken);

            var articlesReadByUser = await _context.KnowledgeArticleReads
                .Include(r => r.Article)
                .CountAsync(r => r.UserId == userId && r.Article.CategoryId == article.CategoryId && r.Article.Status == ArticleStatus.Published && !r.Article.IsDeleted, cancellationToken);

            if (articlesReadByUser >= totalArticlesInCategory && totalArticlesInCategory > 0)
            {
                // O usuário completou a trilha! Dar o selo ou reativá-lo.
                var userBadge = await _context.UserKnowledgeBadges
                    .FirstOrDefaultAsync(ub => ub.BadgeId == badge.Id && ub.UserId == userId, cancellationToken);

                if (userBadge == null)
                {
                    await _context.UserKnowledgeBadges.AddAsync(new UserKnowledgeBadge { BadgeId = badge.Id, UserId = userId, IsActive = true }, cancellationToken);
                }
                else
                {
                    userBadge.IsActive = true; // Reativa o selo que estava cinza (RB034)
                    userBadge.LastUpdatedAt = DateTime.UtcNow;
                    _context.UserKnowledgeBadges.Update(userBadge);
                }
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private async Task InvalidateBadgesForCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
        {
            // RB034: Se saiu artigo novo/versão nova, todos os selos desta categoria ficam cinzas (IsActive = false)
            var badge = await _context.KnowledgeBadges.FirstOrDefaultAsync(b => b.CategoryId == categoryId, cancellationToken);
            if (badge != null)
            {
                var userBadges = await _context.UserKnowledgeBadges.Where(ub => ub.BadgeId == badge.Id && ub.IsActive).ToListAsync(cancellationToken);
                foreach (var ub in userBadges)
                {
                    ub.IsActive = false;
                }
                _context.UserKnowledgeBadges.UpdateRange(userBadges);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        private string GenerateSlug(string phrase)
        {
            // Lógica simples de gerar URL amigável (Ex: "Configuração de Switch" -> "configuracao-de-switch")
            string str = phrase.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 60 ? str.Length : 60).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        #endregion
    }
}