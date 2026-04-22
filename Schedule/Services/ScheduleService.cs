using Schedule.Models;
using Schedule.Data;
using Schedule.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Schedule.Services
{
    public class ScheduleService
    {
        private readonly ApplicationDbContext _context;

        public ScheduleService(ApplicationDbContext context)
         {
             _context = context;
         }
         public async Task GenerateRotationAsync(RotationRequestDTO requestDTO)
        {
            var  totalDays = (requestDTO.EndDate - requestDTO.StartDate).Days + 1;

            var scheduleDaysToInsert = new List<ScheduleDay>();

            for (int i = 0; i < totalDays; i ++)
            {
                var currentDate = requestDTO.StartDate.AddDays(i);
                var rotationIndex = i % requestDTO.ShiftRotationIds.Count;
                var currentShiftId = requestDTO.ShiftRotationIds[rotationIndex];

                var scedule = new ScheduleDay
                {
                    Date = currentDate,
                    LetterId = requestDTO.LetterId,
                    ShiftId = currentShiftId
                }; 

                scheduleDaysToInsert.Add(scedule);
            }

            _context.ScheduleDays.AddRange(scheduleDaysToInsert);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ScheduleResponseDTO>> GetScheduleByMonthAsync(int letterId, int year, int month)
        {
            var schedule = await _context.ScheduleDays
                .Include(s => s.Shift)
                .Where(s => s.LetterId == letterId && s.Date.Month == month && s.Date.Year == year)
                .OrderBy(s => s.Date)
                .Select(s => new ScheduleResponseDTO
                {
                    Id = s.Id,
                    Date = s.Date,
                    ShiftName = s.Shift.Name,
                    StartTime = s.Shift.StartTime,
                    EndTime = s.Shift.EndTime,
                    IsDayOff = s.Shift.IsDayOff

                }).ToListAsync();

            return schedule;

        }


    }
}
