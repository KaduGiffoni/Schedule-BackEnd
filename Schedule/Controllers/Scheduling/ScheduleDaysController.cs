using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Schedule.DTOs;
using Schedule.Services;
using System.Security.Claims;


namespace Schedule.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleDaysController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleDaysController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost("generate-rotation")]
        public async Task<IActionResult> GenerateRotation([FromBody] RotationRequestDTO requestDTO)
        {
            if (requestDTO.StartDate > requestDTO.EndDate)
            {
                return BadRequest("A data de início não pode ser maior que a data de fim.");
            }

            if (requestDTO.ShiftPatternId <= 0)
            {
                return BadRequest("Você precisa informar o Padrão de Turno (ShiftPatternId).");
            }

            await _scheduleService.GenerateRotationAsync(requestDTO);

            return Ok(new { Mensagem = "Escala gerada com sucesso!" });
        }

        [Authorize]
        [HttpGet("letter/{letterId}")]
        public async Task<IActionResult> GetScheduleByMonth(int letterId, [FromQuery] int year, [FromQuery] int month)
        {
            
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized(new { Erro = "Usuário não autenticado." });
            }

            if (month < 1 || month > 12)
            {
                return BadRequest(new { Erro = "Mês inválido. Informe um número de 1 a 12." });
            }

            
            var schedule = await _scheduleService.GetScheduleByMonthAsync(loggedInUserId, letterId, year, month);

             
            if (schedule == null || schedule.Count == 0)
            {
                return NotFound(new { Erro = "Nenhuma escala encontrada para esta letra neste mês." });
            }

            return Ok(schedule);
        }


        [Authorize]
        [HttpGet("escala-geral")]
        public async Task<IActionResult> GetEscalaGeral(
            [FromQuery] int ano,
            [FromQuery] int mes,
            [FromQuery] int? letterId = null,
            [FromQuery] bool apenasFolga = false)
        {
            if (ano <= 2000 || mes < 1 || mes > 12)
                return BadRequest("Ano ou mês inválidos.");

            
            var escala = await _scheduleService.GetEscalaGeralAsync(ano, mes, letterId, apenasFolga);

            return Ok(escala);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpDelete("reset-escala")]
        public async Task<IActionResult> ResetarEscala()
        {
            await _scheduleService.CleanCompleteScheduleAsync();
            return Ok(new { Mensagem = "Escala zerada com sucesso! O banco está limpo." });
        }

    }
}
