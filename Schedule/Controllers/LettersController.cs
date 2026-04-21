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

         [HttpPost]
         public async Task<IActionResult> Createletter (Letter letter)
         {
             _context.Letters.Add(letter);
             await _context.SaveChangesAsync();
             return Ok(letter);
        }

        [HttpGet]

        public async Task<IActionResult> Getletter()
        {
            var letters = await _context.Letters.ToListAsync();
            return Ok(letters);
        }

    }
}
