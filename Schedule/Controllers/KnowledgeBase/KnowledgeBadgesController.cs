using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.Interfaces.KnowledgeBase;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Controllers.KnowledgeBase;

[ApiController]
[Route("api/knowledge-base/badges")]
[Authorize]
public class KnowledgeBadgesController : ControllerBase
{
    private readonly IKnowledgeBaseService _service;

    public KnowledgeBadgesController(IKnowledgeBaseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtém todas as insígnias de gamificação do utilizador.
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBadges(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var badges = await _service.GetMyBadgesAsync(userId, ct);
        return Ok(badges);
    }
}
