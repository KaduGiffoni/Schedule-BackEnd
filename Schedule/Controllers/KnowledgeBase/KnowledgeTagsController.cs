using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.DTOs.KnowledgeBase.Requests;
using Schedule.Interfaces.KnowledgeBase;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Controllers.KnowledgeBase;

[ApiController]
[Route("api/knowledge-base/tags")]
public class KnowledgeTagsController : ControllerBase
{
    private readonly IKnowledgeBaseService _service;

    public KnowledgeTagsController(IKnowledgeBaseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todas as tags disponíveis no sistema.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var tags = await _service.GetAllTagsAsync(ct);
        return Ok(tags);
    }

    /// <summary>
    /// Cria uma nova tag. Restrito a Editores ou Administradores (RB002).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> Create(CreateKnowledgeTagRequest request, CancellationToken ct)
    {
        var tag = await _service.CreateTagAsync(request, ct);
        return Ok(tag);
    }

    /// <summary>
    /// Remove uma tag. Restrito a Administradores (RB002).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteTagAsync(id, ct);
        return NoContent();
    }
}