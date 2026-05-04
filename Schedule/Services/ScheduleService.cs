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
            if (requestDTO.EndDate < requestDTO.StartDate)
                throw new ArgumentException("A data final da escala não pode ser anterior à data inicial.");

            // 1. Pega a fórmula matemática do banco
            var pattern = await _context.ShiftPatterns.FindAsync(requestDTO.ShiftPatternId);
            if (pattern == null)
                throw new ArgumentException("Padrão não encontrado.");

            var shiftRotationIds = pattern.Sequence.Split(',').Select(int.Parse).ToList();

            // 2. Busca TODAS as letras que pertencem a esse Setor
            var letters = await _context.Letters
                .Where(l => l.SectorId == requestDTO.SectorId)
                .ToListAsync();

            if (!letters.Any())
                throw new ArgumentException("Nenhuma letra encontrada para este setor.");

            var totalDays = (requestDTO.EndDate - requestDTO.StartDate).Days + 1;
            var scheduleDaysToInsert = new List<ScheduleDay>();

            // 3. O Loop Duplo: Para cada letra, calcula os dias dela
            foreach (var letter in letters)
            {
                for (int i = 0; i < totalDays; i++)
                {
                    var currentDate = requestDTO.StartDate.AddDays(i);

                    // Pega a defasagem (Offset) específica desta letra
                    var rotationIndex = (i + letter.PatternOffset) % shiftRotationIds.Count;
                    var currentShiftId = shiftRotationIds[rotationIndex];

                    scheduleDaysToInsert.Add(new ScheduleDay
                    {
                        Date = currentDate,
                        LetterId = letter.Id, // A letra atual do loop
                        ShiftId = currentShiftId
                    });
                }
            }

            // 4. Salva a escala inteira do setor (todas as letras) de uma vez só!
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

        public async Task<List<ScheduleResponseDTO>> GetEscalaGeralAsync(int ano, int mes, int? letterId, bool apenasFolga)
        {
            // 1. Prepara a busca trazendo os dados do Turno (Shift) junto
            var query = _context.ScheduleDays
                .Include(s => s.Shift)
                .AsQueryable();

            // 2. Filtro Obrigatório: Ano e Mês
            query = query.Where(s => s.Date.Year == ano && s.Date.Month == mes);

            // 3. Filtro Opcional: Se o Front-end mandou um LetterId, filtra só aquela letra
            if (letterId.HasValue)
            {
                query = query.Where(s => s.LetterId == letterId.Value);
            }

            // 4. Filtro Opcional: Apenas dias de folga
            if (apenasFolga)
            {
                query = query.Where(s => s.Shift.IsDayOff == true);
            }

            // 5. Executa no banco e transforma direto no seu DTO
            var escalaLimpa = await query
                .OrderBy(s => s.Date)
                .ThenBy(s => s.LetterId)
                .Select(day => new ScheduleResponseDTO
                {
                    Id = day.Id,
                    Date = day.Date,
                    LetterId = day.LetterId, // Manda a letra pro Front-end!
                    ShiftName = day.Shift.Name,
                    StartTime = day.Shift.StartTime,
                    EndTime = day.Shift.EndTime,
                    IsDayOff = day.Shift.IsDayOff,

                    // Como é uma visão geral, assumimos que não estamos olhando para trocas específicas de um usuário
                    IsSwapped = false,
                    SwappedWithUserId = null,
                    SwappedWithUserName = null
                })
                .ToListAsync();

            return escalaLimpa;
        }

        public async Task CleanCompleteScheduleAsync()
        {
            await _context.ScheduleDays.ExecuteDeleteAsync();
        }


    }
}
