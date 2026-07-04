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

        // Lança uma nova ausência
        public async Task<UserAbsence> CreateAbsenceAsync(DateTime start, DateTime end, string userId)
        {
            if (start > end)
                throw new ArgumentException("A data de início não pode ser maior que a data de fim.");

            var absence = new UserAbsence
            {
                UserId = userId,
                StartDate = start.Date, // .Date garante que zera as horas (00:00:00)
                EndDate = end.Date,
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

            return await _context.UserAbsences
                .Include(a => a.User)
                .Where(a => a.EndDate >= firstDayOfMonth)
                .OrderBy(a => a.StartDate)
                .Select(a => new AbsenceResponseDTO
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = a.User != null ? a.User.UserName ?? "Usuário" : "Usuário",
                    StartDate = a.StartDate,
                    EndDate = a.EndDate
                })
                .ToListAsync();
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