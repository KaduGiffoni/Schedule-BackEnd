using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Models;

namespace Schedule.Controllers
{   

    [Route("api/[controller]")]
    [ApiController]
    public class LettersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LettersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
         public async Task<IActionResult> Createletter (Letter letter)
         {
             _context.Letters.Add(letter);
             await _context.SaveChangesAsync();
             return Ok(letter);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpGet]

        public async Task<IActionResult> Getletter()
        {
            var letters = await _context.Letters.ToListAsync();
            return Ok(letters);
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLetter(int id, [FromBody] Letter letterUpdate)
        {
         
            if (id != letterUpdate.Id)
            {
                return BadRequest(new { Erro = "O ID da URL não bate com o ID do corpo." });
            }

           
            var letterDb = await _context.Letters.FindAsync(id);

            if (letterDb == null)
            {
                return NotFound(new { Erro = "Letra não encontrada." });
            }

            
            letterDb.Name = letterUpdate.Name;
            letterDb.SectorId = letterUpdate.SectorId;
            letterDb.PatternOffset = letterUpdate.PatternOffset;

            
            await _context.SaveChangesAsync();

            return Ok(new { Mensagem = "Letra atualizada com sucesso!", Letra = letterDb });
        }

    }
}
