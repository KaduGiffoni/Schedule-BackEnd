using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.DTOs;
using Schedule.Models;
using Schedule.Services;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HolidaysController : ControllerBase
    {
        private readonly HolidayService _holidayService;
        private readonly ApplicationDbContext _context;
        public HolidaysController(ApplicationDbContext context, HolidayService holidayService)
        {
            _context = context;
            _holidayService = holidayService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllHolidays()
        {
            int currentYear = DateTime.Now.Year;

            
            bool isHealthy = await _holidayService.CheckHolidaysHealthAsync();

            
            var holidays = await _context.Holidays.OrderBy(h => h.Date).ToListAsync();

            
            return Ok(new
            {
                NeedsSync = !isHealthy, 
                CurrentYear = currentYear,
                Data = holidays
            });
        }

        [HttpPost("sync/{year}")]
        public async Task<IActionResult> SyncHolidays(int year)
        {
            await _holidayService.SyncNationalHolidaysAsync(year);
            return Ok(new { Message = $"Feriados de {year} sincronizados com sucesso!" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateHoliday([FromBody] HolidayCreateDTO request)
        {
            var holiday = new Holiday
            {
                Name = request.Name,
                Date = request.Date,
                Type = request.type,
                IsRecurring = request.IsRecurring
            };
            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllHolidays), new { id = holiday.Id }, holiday);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHoliday(int id, [FromBody] HolidayUpdateDTO request)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null) return NotFound("Feriado não encontrado.");

            holiday.Name = request.Name;
            holiday.Date = request.Date.Date;
            holiday.Type = request.Type;
            holiday.IsRecurring = request.IsRecurring;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Feriado atualizado com sucesso!" });
        }
    }

}