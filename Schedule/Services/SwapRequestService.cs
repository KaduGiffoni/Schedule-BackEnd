using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.DTOs;
using Schedule.Models;


namespace Schedule.Services
{
    public class SwapRequestService
    {
        private readonly ApplicationDbContext _context;

        public SwapRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateSwapRequestAsync(string requestorId, CreateSwapRequestDTO requestDTO)
        {
            var swapRequest = new SwapRequest
            {
                RequestingUserId = requestorId,
                TargetUserId = requestDTO.TargetUserId,
                ScheduleDayId = requestDTO.ScheduleDayId,
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
             
            _context.SwapRequests.Add(swapRequest);
            await _context.SaveChangesAsync();
        }

        public async Task RespondToSwapRequestAsync(int requestId, bool accept)
        {
            var request = await _context.SwapRequests
                .Include(r => r.ScheduleDay)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.Status != RequestStatus.Pending)
                throw new Exception("Solicitação inválida ou já processada.");

            if (accept)
            {
                request.Status = RequestStatus.Approved;

                if (request.ScheduleDay != null)
                {
                    request.ScheduleDay.LetterId = 0; // Lógica temporária
                }
            }
            else
            {
                request.Status = RequestStatus.Rejected;
            }

            await _context.SaveChangesAsync();
        }
    }
}
