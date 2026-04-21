using Schedule.Models;
using Schedule.Data;
using Schedule.DTOs;

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
            }

            _context.ScheduleDays.AddRange(scheduleDaysToInsert);
            await _context.SaveChangesAsync();
        }

        
    }
}
