using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Models;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SectorsController(ApplicationDbContext context)
        {
            _context = context;
        }

         [HttpPost]
         public async Task<IActionResult> CreateSector (Sector sector)
         {
             _context.Sectors.Add(sector);
             await _context.SaveChangesAsync();
             return Ok(sector);
        }

        [HttpGet]

        public async Task<IActionResult> GetSectors()
        {
            var sectors = await _context.Sectors.Include(c => c.Letters).ToListAsync();
            return Ok(sectors);
        }

    }
}
