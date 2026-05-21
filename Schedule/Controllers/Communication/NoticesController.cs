using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.DTOs;
using Schedule.Services;
using System.Security.Claims;

namespace Schedule.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NoticesController : ControllerBase
    {
        private readonly NoticeService _noticeService;

        public NoticesController(NoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        // 1. BUSCAR O MURAL DE AVISOS (Retorna NoticeResponseDTO)
        [HttpGet("my-board")]
        public async Task<IActionResult> GetMyBoard()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            var board = await _noticeService.GetActiveNoticesForUserAsync(loggedInUserId);
            return Ok(board);
        }

        // 2. CRIAR UM AVISO GERAL OU DE TURNO (Usa NoticeCreateDTO)
        [HttpPost]
        public async Task<IActionResult> CreateNotice([FromBody] NoticeCreateDTO request)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            if (request.Type != "Geral" && request.Type != "Turno")
                return BadRequest(new { Erro = "O Tipo deve ser 'Geral' ou 'Turno'." });

            var notice = await _noticeService.CreateNoticeAsync(
                request.Title,
                request.Content,
                request.Type,
                loggedInUserId
            );

            return Ok(new { Mensagem = "Aviso publicado!", Id = notice.Id });
        }

        // 3. DAR O "CIENTE" / RESOLVER
        [HttpPost("{id}/acknowledge")]
        public async Task<IActionResult> AcknowledgeNotice(int id)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            await _noticeService.AcknowledgeNoticeAsync(id, loggedInUserId);
            return Ok(new { Mensagem = "Aviso atualizado com sucesso!" });
        }

        // 4. ADICIONAR UM COMENTÁRIO NA PASSAGEM DE TURNO
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] NoticeCommentCreateDTO request)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { Erro = "O conteúdo do comentário não pode ser vazio." });

            var newComment = await _noticeService.AddCommentAsync(id, request.Content, loggedInUserId);
            return Ok(new { Mensagem = "Comentário adicionado!", Comment = newComment });
        }
    }
}