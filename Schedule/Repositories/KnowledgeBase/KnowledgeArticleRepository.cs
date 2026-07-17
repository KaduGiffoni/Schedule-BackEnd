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

namespace Schedule.Repositories.KnowledgeBase;

public class KnowledgeArticleRepository : IKnowledgeArticleRepository
{
    private readonly ApplicationDbContext _context;

    public KnowledgeArticleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<KnowledgeArticle?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.CurrentVersion)
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Include(a => a.References).ThenInclude(r => r.ReferencedArticle).ThenInclude(ra => ra.CurrentVersion)
            .AsSplitQuery() // Otimização crítica: Evita Cartesian Explosion devido a múltiplos Includes de coleções
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, ct);
    }

    public async Task<KnowledgeArticle?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _context.KnowledgeArticles
            .Include(a => a.CurrentVersion)
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Include(a => a.References).ThenInclude(r => r.ReferencedArticle).ThenInclude(ra => ra.CurrentVersion)
            .AsSplitQuery() // Otimização crítica: Evita Cartesian Explosion
            .FirstOrDefaultAsync(a => a.Slug == slug && !a.IsDeleted, ct);
    }

    public async Task<(IEnumerable<KnowledgeArticle> Articles, int TotalCount)> SearchAsync(
        string? searchTerm,
        Guid? categoryId,
        IEnumerable<Guid>? tagIds,
        ArticleStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        // Otimização: AsNoTracking é essencial aqui pois é apenas leitura para pesquisa/listagem
        var query = _context.KnowledgeArticles
            .AsNoTracking()
            .Include(a => a.CurrentVersion)
            .Include(a => a.Category)
            .Include(a => a.Author)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        if (tagIds != null && tagIds.Any())
        {
            query = query.Where(a => a.ArticleTags.Any(at => tagIds.Contains(at.TagId)));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Full-Text Search simulado (Pode ser melhorado com índices FTS reais da DB no futuro)
            query = query.Where(a =>
                (a.CurrentVersion != null && a.CurrentVersion.Title.Contains(searchTerm)) ||
                (a.CurrentVersion != null && a.CurrentVersion.Summary.Contains(searchTerm)));
        }

        int totalCount = await query.CountAsync(ct);

        var articles = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (articles, totalCount);
    }

    public async Task<KnowledgeArticle> AddAsync(KnowledgeArticle article, CancellationToken ct = default)
    {
        await _context.KnowledgeArticles.AddAsync(article, ct);
        await _context.SaveChangesAsync(ct);
        return article;
    }

    public async Task UpdateAsync(KnowledgeArticle article, CancellationToken ct = default)
    {
        _context.KnowledgeArticles.Update(article);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken ct = default)
    {
        var article = await _context.KnowledgeArticles.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (article != null)
        {
            article.IsDeleted = true;
            _context.KnowledgeArticles.Update(article);
            await _context.SaveChangesAsync(ct);
        }
    }
}