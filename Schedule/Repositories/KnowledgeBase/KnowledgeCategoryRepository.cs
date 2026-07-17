using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Interfaces.KnowledgeBase;
using Schedule.Models.KnowledgeBase;

namespace Schedule.Repositories.KnowledgeBase;

/// <summary>
/// Implementação do repositório de Categorias da Base de Conhecimento.
/// Isola as consultas do EF Core e gerencia a estrutura hierárquica (RB021).
/// </summary>
public class KnowledgeCategoryRepository : IKnowledgeCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public KnowledgeCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<KnowledgeCategory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<KnowledgeCategory>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        // Busca apenas as categorias "Raiz" (que não possuem pai) 
        // e realiza o Include das Subcategorias (RB021).
        // Adicionado AsNoTracking() pois árvores de UI são estritamente para leitura e visualização.
        return await _context.KnowledgeCategories
            .AsNoTracking()
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.SubCategories)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<KnowledgeCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeCategories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<KnowledgeCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeCategories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<KnowledgeCategory> AddAsync(KnowledgeCategory category, CancellationToken cancellationToken = default)
    {
        await _context.KnowledgeCategories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task UpdateAsync(KnowledgeCategory category, CancellationToken cancellationToken = default)
    {
        _context.KnowledgeCategories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(KnowledgeCategory category, CancellationToken cancellationToken = default)
    {
        _context.KnowledgeCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}