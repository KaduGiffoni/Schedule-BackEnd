using System.Text.Json;
using Schedule.Data;
using Schedule.Models;
using Schedule.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Schedule.Services
{
    public class HolidayService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public HolidayService(ApplicationDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task SyncNationalHolidaysAsync(int year)
        {
            var response = await _httpClient.GetAsync($"https://brasilapi.com.br/api/feriados/v1/{year}");

            if (!response.IsSuccessStatusCode) return;

            var jsonString = await response.Content.ReadAsStringAsync();
            var apiHolidays = JsonSerializer.Deserialize<List<BrasilApiHolidayDTO>>(jsonString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (apiHolidays == null) return;

            // 2. Varre os feriados retornados
            foreach (var apiHoliday in apiHolidays)
            {
                var holidayDate = DateTime.Parse(apiHoliday.Date);

                // 3. Verifica se já existe no banco para não duplicar
                var exists = await _context.Holidays.AnyAsync(h => h.Date.Date == holidayDate.Date);

                if (!exists)
                {
                    _context.Holidays.Add(new Holiday
                    {
                        Name = apiHoliday.Name,
                        Date = holidayDate,
                        Type = "Nacional"
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        
        public async Task ReplicateRecurringHolidaysAsync(int targetYear)
        {
            var recurringHolidays = await _context.Holidays
                .Where(h => h.IsRecurring && h.Date.Year == targetYear - 1)
                .ToListAsync();

            foreach (var oldHoliday in recurringHolidays)
            {
                var newDate = new DateTime(targetYear, oldHoliday.Date.Month, oldHoliday.Date.Day);

                if (!await _context.Holidays.AnyAsync(h => h.Date == newDate && h.Name == oldHoliday.Name))
                {
                    _context.Holidays.Add(new Holiday
                    {
                        Name = oldHoliday.Name,
                        Date = newDate,
                        Type = oldHoliday.Type,
                        IsRecurring = true
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        // 2. O "Vigia": Verifica se o ano atual tem feriados carregados
        public async Task<bool> CheckHolidaysHealthAsync()
        {
            int currentYear = DateTime.Now.Year;
            return await _context.Holidays.AnyAsync(h => h.Date.Year == currentYear);
        }
    }
}