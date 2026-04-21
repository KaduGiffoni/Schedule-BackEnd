using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule.DTOs;
using Schedule.Services;


namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleDaysController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;

        public ScheduleDaysController(ScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpPost("generate-rotation")]
        public async Task<IActionResult> GenerateRotation([FromBody] RotationRequestDTO requestDTO)
        {
            if (requestDTO.StartDate > requestDTO.EndDate)
            {
                return BadRequest("A data de início não pode ser maior que a data de fim.");
            }
            if (requestDTO.ShiftRotationIds == null || requestDTO.ShiftRotationIds.Count == 0)
            {
                return BadRequest("Você precisa informar a sequência de turnos da rotação.");
            }
            await _scheduleService.GenerateRotationAsync(requestDTO);

            return Ok(new { Mensagem = "Escala gerada com sucesso!" });
        }

    }
}
