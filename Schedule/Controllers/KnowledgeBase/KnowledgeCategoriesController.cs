using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.DTOs.KnowledgeBase.Requests;
using Schedule.Interfaces.KnowledgeBase;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Schedule.Controllers.KnowledgeBase;

[ApiController]
[Route("api/knowledge-base/categories")]
public class KnowledgeCategoriesController : ControllerBase
{
    private readonly IKnowledgeBaseService _service;

    public KnowledgeCategoriesController(IKnowledgeBaseService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtém a árvore de categorias para o menu lateral (RB021).
    /// </summary>
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree(CancellationToken ct)
    {
        var tree = await _service.GetCategoryTreeAsync(ct);
        return Ok(tree);
    }

    /// <summary>
    /// Cria uma nova categoria. Restrito a Administradores (RB002).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateKnowledgeCategoryRequest request, CancellationToken ct)
    {
        var category = await _service.CreateCategoryAsync(request, ct);
        return CreatedAtAction(nameof(GetTree), new { id = category.Id }, category);
    }

    /// <summary>
    /// Atualiza uma categoria existente. Restrito a Administradores.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, UpdateKnowledgeCategoryRequest request, CancellationToken ct)
    {
        if (id != request.Id) return BadRequest("ID da rota diferente do corpo da requisição.");

        var category = await _service.UpdateCategoryAsync(request, ct);
        return Ok(category);
    }

    /// <summary>
    /// Remove uma categoria. Restrito a Administradores.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _service.DeleteCategoryAsync(id, ct);
        return NoContent();
    }
}