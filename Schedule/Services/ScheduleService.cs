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

        public async Task<List<ScheduleResponseDTO>> GetScheduleByMonthAsync(string userId, int letterId, int year, int month)
        {
            var schedule = await _context.ScheduleDays
            .Include(s => s.Shift)
            .Where(s => s.LetterId == letterId && s.Date.Month == month && s.Date.Year == year)
            .OrderBy(s => s.Date)
            .ToListAsync();


            var dayIds = schedule.Select(s => s.Id).ToList();

            var approvedSwaps = await _context.SwapRequests
                .Where(sr => dayIds.Contains(sr.ScheduleDayId)
                          && sr.Status == RequestStatus.Approved
                          && (sr.RequestingUserId == userId || sr.TargetUserId == userId))
                .ToListAsync();

            var response = new List<ScheduleResponseDTO>();

            foreach (var day in schedule)
            {
               
                var dto = new ScheduleResponseDTO
                {
                    Id = day.Id,
                    Date = day.Date,
                    ShiftName = day.Shift.Name,
                    StartTime = day.Shift.StartTime,
                    EndTime = day.Shift.EndTime,
                    IsDayOff = day.Shift.IsDayOff,
                    IsSwapped = false,
                    SwappedWithUserId = null
                };

               
                var swap = approvedSwaps.FirstOrDefault(sr => sr.ScheduleDayId == day.Id);

                if (swap != null)
                {
                   
                    dto.IsSwapped = true;


                    dto.SwappedWithUserId = swap.RequestingUserId == userId
                        ? swap.TargetUserId
                        : swap.RequestingUserId;
                }

                response.Add(dto);
            }

            return response;

        }


    }
}
