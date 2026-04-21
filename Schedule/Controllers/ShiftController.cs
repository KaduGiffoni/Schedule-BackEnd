using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.Models;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShiftController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateShift (Shift shift)
        {
            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();
            return Ok(shift);
        }

         [HttpGet]
         public async Task<IActionResult> GetShifts()
         {
             var shifts = await _context.Shifts.ToListAsync();
             return Ok(shifts);
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateShift(int id, Shift shift)
        {
            if (id != shift.Id)
            {
                return BadRequest("ID do turno não corresponde ao ID fornecido.");
            }
            
            _context.Entry(shift).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Shifts.Any(e => e.Id == id))
                {
                    return NotFound("Turno não encontrado para atualização.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

    }
}
