using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Interfaces.KnowledgeBase;
using Schedule.Models.KnowledgeBase;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Repositories.KnowledgeBase
{
    /// <summary>
    /// Implementação do repositório de Artigos da Base de Conhecimento.
    /// Isola as consultas do Entity Framework e otimiza buscas pesadas.
    /// </summary>
    public class KnowledgeArticleRepository : IKnowledgeArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public KnowledgeArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KnowledgeArticle?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.KnowledgeArticles
                .Include(a => a.Category)
                .Include(a => a.Author)
                .Include(a => a.CurrentVersion)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
        }

        public async Task<KnowledgeArticle?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await _context.KnowledgeArticles
                .Include(a => a.Category)
                .Include(a => a.Author)
                .Include(a => a.CurrentVersion)
                .FirstOrDefaultAsync(a => a.Slug == slug && !a.IsDeleted, cancellationToken);
        }

        public async Task<(IEnumerable<KnowledgeArticle> Articles, int TotalCount)> SearchAsync(
            string? searchTerm,
            Guid? categoryId,
            IEnumerable<Guid>? tagIds,
            ArticleStatus? status,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // Inicia a query base filtrando os deletados logicamente
            var query = _context.KnowledgeArticles
                .Include(a => a.Category)
                .Include(a => a.CurrentVersion)
                .Where(a => !a.IsDeleted)
                .AsNoTracking(); // Otimização: Apenas leitura, não precisa rastrear alterações

            // Filtro de Status
            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            // Filtro de Categoria
            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            // Filtro de Tags (Trabalhando com a tabela de junção)
            if (tagIds != null && tagIds.Any())
            {
                query = query.Where(a => _context.KnowledgeArticleTags
                                         .Any(kat => kat.ArticleId == a.Id && tagIds.Contains(kat.TagId)));
            }

            // Pesquisa Textual Otimizada (Full-Text Search)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // NOTA: EF.Functions.FreeText exige que o índice Full-Text esteja configurado no SQL Server.
                // Isso garante uma busca que ignora acentos, maiúsculas e busca por partes do texto.
                query = query.Where(a =>
                    EF.Functions.FreeText(a.Title, searchTerm) ||
                    (a.CurrentVersion != null && EF.Functions.FreeText(a.CurrentVersion.Summary, searchTerm)));
            }

            // Ordenação padrão (mais recentes ou mais acessados primeiro)
            query = query.OrderByDescending(a => a.CreatedAt);

            // Conta o total de registros ANTES de paginar
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplica a Paginação
            var articles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (articles, totalCount);
        }

        public async Task<KnowledgeArticle> AddAsync(KnowledgeArticle article, CancellationToken cancellationToken = default)
        {
            await _context.KnowledgeArticles.AddAsync(article, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return article;
        }

        public async Task UpdateAsync(KnowledgeArticle article, CancellationToken cancellationToken = default)
        {
            _context.KnowledgeArticles.Update(article);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var article = await _context.KnowledgeArticles.FindAsync(new object[] { id }, cancellationToken);
            if (article != null)
            {
                article.IsDeleted = true; // Aplicação da Regra de Negócio de Soft Delete
                _context.KnowledgeArticles.Update(article);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}