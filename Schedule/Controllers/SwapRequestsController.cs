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

            try
            {
                await _swapService.CreateSwapRequestAsync(loggedInUserId, requestDTO);
                return Ok(new { Mensagem = "Solicitação de troca enviada com sucesso!" });
            }catch (InvalidOperationException ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Erro = "Ocorreu um erro interno no servidor." });
            }
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized(new { Erro = "Usuário não autenticado." });
            }

            var requests = await _swapService.GetPendingRequestsAsync(loggedInUserId);

            return Ok(requests);
        }


        [HttpPut("{id}/respond")]
        public async Task<IActionResult> RespondToSwapRequest(int id, [FromQuery] bool accept)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
            { 
                return Unauthorized(new { Erro = "Usuário não autenticado." });
            }
            try
            {
                await _swapService.RespondToRequestAsync(id, loggedInUserId, accept);

                return Ok(new { Mensagem = "Resposta à solicitação registrada com sucesso!" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Erro = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetSwapHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized(new { Erro = "Usuário não autenticado." });
            }

            
            if (page < 1) page = 1;
            if (pageSize > 50) pageSize = 50;

            var result = await _swapService.GetUserSwapHistoryAsync(loggedInUserId, page, pageSize);

            return Ok(result);
        }
    }
}