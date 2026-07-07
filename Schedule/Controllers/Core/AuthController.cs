using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Schedule.Models;
using System.Data;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("user-promoter")]
        public async Task<IActionResult> PromoverUsuario(string email, string cargo)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("Usuário não encontrado no banco de dados.");

            if (!await _roleManager.RoleExistsAsync(cargo))
            {
                return BadRequest($"O cargo '{cargo}' é inválido. Utilize apenas: Admin, Manager, Standard ou Viewer.");
            }

            var result = await _userManager.AddToRoleAsync(user, cargo);

            if (result.Succeeded)
                return Ok(new { Mensagem = $"Sucesso! O usuário {email} agora tem acesso de {cargo}." });

            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("listar-cargos")]
        public IActionResult ListarCargos()
        {
            // Vai no banco e pega apenas o Nome de todos os cargos que existem lá
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            if (!roles.Any())
                return Ok("O banco de dados está VAZIO. Nenhum cargo foi criado.");

            return Ok(roles);
        }


    }
}