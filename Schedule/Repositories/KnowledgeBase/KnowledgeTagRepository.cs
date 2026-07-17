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
/// Implementação do repositório de Tags da Base de Conhecimento.
/// Gerencia as palavras-chave garantindo acesso otimizado aos dados.
/// </summary>
public class KnowledgeTagRepository : IKnowledgeTagRepository
{
    private readonly ApplicationDbContext _context;

    public KnowledgeTagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<KnowledgeTag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeTags
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<KnowledgeTag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeTags
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<KnowledgeTag?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.KnowledgeTags
            .FirstOrDefaultAsync(t => t.Slug == slug, cancellationToken);
    }

    public async Task<KnowledgeTag> AddAsync(KnowledgeTag tag, CancellationToken cancellationToken = default)
    {
        await _context.KnowledgeTags.AddAsync(tag, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return tag;
    }

    public async Task UpdateAsync(KnowledgeTag tag, CancellationToken cancellationToken = default)
    {
        _context.KnowledgeTags.Update(tag);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(KnowledgeTag tag, CancellationToken cancellationToken = default)
    {
        _context.KnowledgeTags.Remove(tag);
        await _context.SaveChangesAsync(cancellationToken);
    }
}