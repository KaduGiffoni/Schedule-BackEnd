using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.Services;
using System.Security.Claims;

namespace Schedule.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly NoticeService _noticeService;

        public NotificationsController(NoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        // GET /api/notifications              -> todas
        // GET /api/notifications?onlyUnread=true -> só as não lidas (pro sininho)
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] bool onlyUnread = false)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            var notifications = await _noticeService.GetNotificationsForUserAsync(loggedInUserId, onlyUnread);
            return Ok(notifications);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            try
            {
                var success = await _noticeService.MarkNotificationAsReadAsync(id, loggedInUserId);
                if (!success)
                    return NotFound(new { Erro = "Notificação não encontrada." });

                return Ok(new { Mensagem = "Notificação marcada como lida." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Erro = ex.Message });
            }
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            await _noticeService.MarkAllNotificationsAsReadAsync(loggedInUserId);
            return Ok(new { Mensagem = "Todas as notificações foram marcadas como lidas." });
        }
    }
}