using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.Models;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPut("link-profile")]
        public async Task<IActionResult> AtualizarPerfil(string email, int letterId, string completeName, string registration)
        {

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("Usuário não encontrado. Verifique o e-mail digitado.");
            }

            user.LetterId = letterId;
            user.CompleteName = completeName;
            user.Registration = registration;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Message = "Perfil atualizado com sucesso.",
                    User = user.Email,
                    LetterId = user.LetterId,
                });

            }
            return BadRequest(result.Errors);

        }

        [HttpGet("get-user")]

        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("Usuário não encontrado. Verifique o e-mail digitado.");
            }
            return Ok(new
            {
                UserId = user.Id,
                User = user.Email,
                LetterId = user.LetterId,
                CompleteName = user.CompleteName,
                Registration = user.Registration
            });

        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.Select(u => new
            {
                UserId = u.Id,
                User = u.Email,
                LetterId = u.LetterId,
                CompleteName = u.CompleteName,
                Registration = u.Registration
            }).ToListAsync();
            return Ok(users);

        }
    }
}
