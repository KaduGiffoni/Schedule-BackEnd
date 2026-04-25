using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.DTOs;
using Schedule.Services;

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
        public async Task<IActionResult> CreateRequest([FromQuery] string meuId, [FromBody] CreateSwapRequestDTO dto)
        {

            await _swapService.CreateSwapRequestAsync(meuId, dto);
            return Ok(new { Mensagem = "Solicitação enviada com sucesso para aprovação!" });
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