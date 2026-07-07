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
    public class AbsencesController : ControllerBase
    {
        private readonly AbsenceService _absenceService;

        public AbsencesController(AbsenceService absenceService)
        {
            _absenceService = absenceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAbsences()
        {
            var absences = await _absenceService.GetUpcomingAbsencesAsync();
            return Ok(absences);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAbsence([FromBody] AbsenceCreateDTO request)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            // Admin e Manager podem lançar/apagar ausência de qualquer pessoa.
            // Um usuário comum só pode mexer nos próprios registros.
            var isPrivilegedUser = User.IsInRole("Admin") || User.IsInRole("Manager");

            try
            {
                var absence = await _absenceService.CreateAbsenceAsync(request, loggedInUserId, isPrivilegedUser);
                return Ok(new { Mensagem = "Ausência registrada com sucesso!", Id = absence.Id });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbsence(int id)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
                return Unauthorized(new { Erro = "Usuário não autenticado." });

            var isPrivilegedUser = User.IsInRole("Admin") || User.IsInRole("Manager");

            try
            {
                await _absenceService.DeleteAbsenceAsync(id, loggedInUserId, isPrivilegedUser);
                return Ok(new { Mensagem = "Registro removido com sucesso." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Erro = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Erro = ex.Message });
            }
        }
    }
}