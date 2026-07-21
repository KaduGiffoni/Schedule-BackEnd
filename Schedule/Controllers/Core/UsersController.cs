using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.DTOs;
using Schedule.Models;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<IActionResult> AtualizarPerfil([FromBody] UpdateProfileDTO request)
        {
            
            var emailLogado = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;

            
            bool isAdmin = User.IsInRole("Admin");

            
            if (emailLogado != request.Email && !isAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Message = "Acesso negado. Você só pode alterar o seu próprio perfil."
                });
            }

            
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("Usuário não encontrado. Verifique o e-mail digitado.");
            }

            user.LetterId = request.LetterId;
            user.CompleteName = request.CompleteName;
            user.Surname = request.Surname;
            user.Registration = request.Registration;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new
                {
                    Message = "Perfil atualizado com sucesso.",
                    User = user.Email,
                    LetterId = user.LetterId,
                    CompleteName = user.CompleteName,
                    Surname = user.Surname
                });
            }

            return BadRequest(result.Errors);
        }

        [Authorize]
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
                Surname = user.Surname, 
                Registration = user.Registration
            });

        }

        [HttpGet("get-all-users")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.Select(u => new
            {
                UserId = u.Id,
                User = u.Email,
                LetterId = u.LetterId,
                CompleteName = u.CompleteName,
                Surname = u.Surname,
                Registration = u.Registration

            }).ToListAsync();
            return Ok(users);

        }

        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            
            var emailLogado = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name;
            bool isAdmin = User.IsInRole("Admin");

            
            if (emailLogado != request.Email && !isAdmin)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    Message = "Acesso negado. Você só pode alterar a sua própria senha."
                });
            }

            
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Usuário não encontrado." });
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Senha alterada com sucesso!" });
            }

           
            return BadRequest(result.Errors);
        }

        [HttpPut("admin-reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminResetPassword([FromBody] Schedule.DTOs.Core.AdminResetPasswordDTO request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Usuário não encontrado." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { Message = "Senha do usuário alterada com sucesso pelo Administrador!" });
            }

            return BadRequest(result.Errors);
        }
    }
}
