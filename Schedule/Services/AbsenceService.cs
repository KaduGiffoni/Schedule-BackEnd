using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.DTOs;
using Schedule.Models;

namespace Schedule.Services
{
    public class AbsenceService
    {
        private readonly ApplicationDbContext _context;

        public AbsenceService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lança uma nova ausência (férias, compensação de hora, atestado, etc.)
        public async Task<UserAbsence> CreateAbsenceAsync(AbsenceCreateDTO request, string requesterUserId)
        {
            if (request.StartDate.Date > request.EndDate.Date)
                throw new ArgumentException("A data de início não pode ser maior que a data de fim.");

            // Se mandaram um ID no request, usa ele (Manager inserindo para outro).
            // Se não, pega o ID de quem está logado (Auto-inserção).
            var targetUserId = string.IsNullOrEmpty(request.TargetUserId)
                ? requesterUserId
                : request.TargetUserId;

            var targetUserExists = await _context.Users.AnyAsync(u => u.Id == targetUserId);
            if (!targetUserExists)
                throw new ArgumentException("Usuário informado não encontrado.");

            if (!string.IsNullOrEmpty(request.SubstituteUserId))
            {
                if (request.SubstituteUserId == targetUserId)
                    throw new ArgumentException("O substituto não pode ser a mesma pessoa que está ausente.");

                var substituteExists = await _context.Users.AnyAsync(u => u.Id == request.SubstituteUserId);
                if (!substituteExists)
                    throw new ArgumentException("Usuário substituto não encontrado.");
            }

            var absence = new UserAbsence
            {
                UserId = targetUserId,
                Type = request.Type,
                StartDate = request.StartDate.Date, // .Date garante que zera as horas (00:00:00)
                EndDate = request.EndDate.Date,
                SubstituteUserId = request.SubstituteUserId,
                Notes = request.Notes,
                CreatedAt = DateTime.Now
            };

            _context.UserAbsences.Add(absence);
            await _context.SaveChangesAsync();

            return absence;
        }

        // Busca todas as ausências ativas a partir do mês atual para frente
        public async Task<List<AbsenceResponseDTO>> GetUpcomingAbsencesAsync()
        {
            var today = DateTime.Now.Date;
            // Pega do início do mês atual para não sumir com férias que começaram dia 01
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var absences = await _context.UserAbsences
                .Include(a => a.User)
                .Include(a => a.SubstituteUser)
                .Where(a => a.EndDate >= firstDayOfMonth)
                .OrderBy(a => a.StartDate)
                .ToListAsync();

            return absences.Select(a => new AbsenceResponseDTO
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.User != null ? a.User.UserName ?? "Usuário" : "Usuário",
                Type = a.Type,
                TypeDescription = a.Type.ToDisplayName(),
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                TotalDays = a.TotalDays,
                SubstituteUserId = a.SubstituteUserId,
                SubstituteUserName = a.SubstituteUser?.UserName,
                Notes = a.Notes
            }).ToList();
        }

        // Deleta (caso alguém lance errado)
        public async Task DeleteAbsenceAsync(int id)
        {
            var absence = await _context.UserAbsences.FindAsync(id);
            if (absence != null)
            {
                _context.UserAbsences.Remove(absence);
                await _context.SaveChangesAsync();
            }
        }
    }
}