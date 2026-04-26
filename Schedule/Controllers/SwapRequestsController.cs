using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.DTOs;
using Schedule.Models;
using Schedule.Services;
using System.Security.Claims;

namespace Schedule.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SwapRequestsController : ControllerBase
    {
        private readonly SwapRequestService _swapService;

        public SwapRequestsController(SwapRequestService swapService)
        {
            _swapService = swapService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSwapRequest([FromBody] CreateSwapRequestDTO requestDTO)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized(new { Erro = "Usuário não autenticado." });
            }

            var request = new SwapRequest
            {
                RequestingUserId = loggedInUserId,
                TargetUserId = requestDTO.TargetUserId,
                ScheduleDayId = requestDTO.ScheduleDayId
            };

            await _swapService.CreateSwapRequestAsync(loggedInUserId, requestDTO);

            return Ok("Solicitação de troca enviada com sucesso!");
        }

        [HttpGet("pending/{userId}")]
        public async Task<IActionResult> GetPendingRequests(string userId)
        {
            var pending = await _swapService.GetPendingRequestsAsync(userId);

            if (pending.Count == 0)
            {
                return NotFound("Nenhuma solicitação pendente encontrada.");
            }

            return Ok(pending);
        }


        [HttpPut("{id}/respond")]
        public async Task<IActionResult> RespondToRequest(int id, [FromQuery] bool accept)
        {
            try
            {
                await _swapService.RespondToSwapRequestAsync(id, accept);

                var status = accept ? "aceita" : "recusada";
                return Ok(new { Mensagem = $"A troca de turno foi {status} com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }
    }
}