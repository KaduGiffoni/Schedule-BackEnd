using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.Interfaces.KnowledgeBase;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Controllers.KnowledgeBase;

[ApiController]
[Route("api/knowledge-base/interactions")]
[Authorize] // RB001: Apenas utilizadores autenticados podem interagir
public class KnowledgeInteractionsController : ControllerBase
{
    private readonly IKnowledgeBaseService _service;

    public KnowledgeInteractionsController(IKnowledgeBaseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Regista que o utilizador visualizou o artigo (RB015, RB024).
    /// </summary>
    [HttpPost("{articleId:guid}/view")]
    public async Task<IActionResult> RegisterView(Guid articleId, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _service.RegisterViewAsync(articleId, userId, ct);
        return Ok();
    }

    /// <summary>
    /// Alterna o estado de favorito do artigo para o utilizador (RB014, RB025).
    /// </summary>
    [HttpPost("{articleId:guid}/favorite")]
    public async Task<IActionResult> ToggleFavorite(Guid articleId, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _service.ToggleFavoriteAsync(articleId, userId, ct);
        return Ok();
    }

    /// <summary>
    /// Marca o artigo como lido/concluído pelo utilizador (RB032).
    /// Dispara a lógica de Gamificação.
    /// </summary>
    [HttpPost("{articleId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid articleId, CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        bool badgeUnlocked = await _service.MarkArticleAsReadAsync(articleId, userId, ct);
        return Ok(new { isRead = true, badgeUnlocked = badgeUnlocked });
    }
}