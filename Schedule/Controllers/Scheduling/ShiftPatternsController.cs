using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Models;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftPatternsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShiftPatternsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> CreatePattern([FromBody] ShiftPattern pattern)
        {
            _context.ShiftPatterns.Add(pattern);
            await _context.SaveChangesAsync();
            return Ok(new { Mensagem = "Padrão criado com sucesso!", Padrao = pattern });
        }

        [HttpGet]
        public async Task<IActionResult> GetPatterns()
        {
            var patterns = await _context.ShiftPatterns.ToListAsync();
            return Ok(patterns);
        }
    }
}