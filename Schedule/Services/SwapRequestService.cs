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

        public async Task CreateSwapRequestAsync(string requestingUserId, CreateSwapRequestDTO requestDTO)
        {
            var scheduleDay = await _context.ScheduleDays.FindAsync(requestDTO.ScheduleDayId);

            if (scheduleDay == null)
            {
                throw new Exception("Dia de escala não encontrado no sistema.");
            }

            if (scheduleDay.Date.Date < DateTime.Today)
            {
                throw new InvalidOperationException("Falha na solicitação: Não é possível solicitar troca para um plantão que já passou.");
            }

            var existingRequest = await _context.SwapRequests
                .FirstOrDefaultAsync(sr => sr.RequestingUserId == requestingUserId
                                        && sr.ScheduleDayId == requestDTO.ScheduleDayId
                                        && sr.Status == RequestStatus.Pending);
            if (existingRequest != null)
            {
    
                throw new InvalidOperationException("Falha na solicitação: Você já enviou um pedido de troca para este dia que ainda está aguardando resposta.");
            }

            
            var request = new SwapRequest
            {
                RequestingUserId = requestingUserId,
                TargetUserId = requestDTO.TargetUserId,
                ScheduleDayId = requestDTO.ScheduleDayId,
                Status = RequestStatus.Pending
            };

            _context.SwapRequests.Add(request);
            await _context.SaveChangesAsync();
            
            var targetUser = await _context.Users.FindAsync(requestDTO.TargetUserId);
            if (targetUser == null)
            {
                throw new Exception("Usuário destino não encontrado.");
            }

            var isTargetUserAlreadyWorking = await _context.ScheduleDays
                .Include(sd => sd.Shift)
                .AnyAsync(sd =>
                    sd.LetterId == targetUser.LetterId &&
                    sd.Date.Date == scheduleDay.Date.Date &&
                    sd.Shift.IsDayOff == false
                );
            if (isTargetUserAlreadyWorking)
            {
                throw new InvalidOperationException("Falha na solicitação: O colega selecionado não pode assumir este turno pois já está escalado para trabalhar neste mesmo dia.");
            }

            var request = new SwapRequest


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
            }
            else
            {
                request.Status = RequestStatus.Rejected;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<SwapRequest>> GetPendingRequestsAsync(string targetUserId)
        {
            return await _context.SwapRequests
                .Include(r => r.ScheduleDay)
                .ThenInclude(sd => sd.Shift) 
                .Where(r => r.TargetUserId == targetUserId && r.Status == RequestStatus.Pending)
                .ToListAsync();
        }

        public async Task RespondToRequestAsync(int requestId, string userId, bool accept)
        {
            var request = await _context.SwapRequests
                                        .Include(r => r.ScheduleDay)
                                        .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                throw new Exception("Solicitação não encontrada.");
            }

            if (request.TargetUserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para responder a esta solicitação. Ela não foi enviada para você.");
            }

            if (accept)
            {
                request.Status = RequestStatus.Approved;
            }
            else
            {
                request.Status = RequestStatus.Rejected;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<SwapRequest>> GetUserSwapHistoryAsync(string userId, int pageNumber, int pageSize)
        {
          
            var query = _context.SwapRequests
                .Include(sr => sr.ScheduleDay)
                .Where(sr => sr.RequestingUserId == userId || sr.TargetUserId == userId)
                .OrderByDescending(sr => sr.CreatedAt);

            
            var totalItems = await query.CountAsync();

            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            
            return new PagedResult<SwapRequest>
            {
                Items = items,
                TotalCount = totalItems,
                CurrentPage = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
