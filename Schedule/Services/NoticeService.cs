using Microsoft.EntityFrameworkCore;
using Schedule.Data;
using Schedule.DTOs;
using Schedule.Models;

namespace Schedule.Services
{
    public class NoticeService
    {
        private readonly ApplicationDbContext _context;

        public NoticeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. CRIAR UM NOVO AVISO
        // ==========================================
        public async Task<Notice> CreateNoticeAsync(string title, string content, string type, string userId)
        {
            var creator = await _context.Users.FindAsync(userId);

            var sectorId = await GetSectorIdForUserAsync(creator);

            var notice = new Notice
            {
                Title = title,
                Content = content,
                Type = type,
                Status = "Ativo",
                CreatedByUserId = userId,
                CreatedByUser = creator,
                SectorId = sectorId, // Isola o aviso dentro do setor de quem criou
                CreatedAt = DateTime.Now
            };

            _context.Notices.Add(notice);
            await _context.SaveChangesAsync();

            // A MÁGICA ACONTECE AQUI:
            if (type == "Turno")
            {
                await NotifyNextShiftAsync(notice, sectorId);
            }

            return notice;
        }

        // ==========================================
        // 2. DAR O VISTO / RESOLVER PASSAGEM DE TURNO
        // ==========================================
        public async Task AcknowledgeNoticeAsync(int noticeId, string userId)
        {
            var notice = await _context.Notices.FindAsync(noticeId);
            if (notice == null) return;

            // Registra que o usuário Kadu visualizou/confirmou
            var ack = new NoticeAcknowledgment
            {
                NoticeId = noticeId,
                UserId = userId,
                AcknowledgedAt = DateTime.Now
            };

            // Certifique-se de usar o nome exato do DbSet que você colocou no ApplicationDbContext
            _context.NoticeByIdAcknowledgments.Add(ack);

            // A Mágica do Turno: Se for passagem de plantão, o "Visto" significa que o cara resolveu o problema!
            if (notice.Type == "Turno")
            {
                notice.Status = "Resolvido";
            }

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // 3. BUSCAR AVISOS ATIVOS PARA O PAINEL DO USUÁRIO
        // ==========================================
        public async Task<List<NoticeResponseDTO>> GetActiveNoticesForUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var userSectorId = await GetSectorIdForUserAsync(user);

            var notices = await _context.Notices
                .Include(n => n.CreatedByUser)
                .Include(n => n.Comments)
                    .ThenInclude(c => c.User) // Traz quem comentou
                .Where(n =>
                    // Isolamento por setor: só vê avisos do próprio setor,
                    // ou avisos sem setor definido (globais/administrativos)
                    (n.SectorId == null || n.SectorId == userSectorId)
                    &&
                    (
                        (n.Type == "Geral" && !n.Acknowledgments.Any(a => a.UserId == userId))
                        ||
                        // "Turno" só aparece pra quem realmente foi notificado como próximo turno
                        (n.Type == "Turno" && n.Status == "Ativo"
                            && _context.Notifications.Any(no => no.ReferenceNoticeId == n.Id && no.TargetUserId == userId))
                    )
                )
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NoticeResponseDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    Type = n.Type,
                    Status = n.Status,
                    CreatedAt = n.CreatedAt,
                    CreatedByUserName = n.CreatedByUser != null ? n.CreatedByUser.UserName ?? "Usuário" : "Usuário",
                    Comments = n.Comments.Select(c => new NoticeCommentResponseDTO
                    {
                        Id = c.Id,
                        Content = c.Content,
                        CreatedAt = c.CreatedAt,
                        CreatedByUserName = c.User != null ? c.User.UserName ?? "Usuário" : "Usuário"
                    }).ToList()
                })
                .ToListAsync();

            return notices;
        }

        private async Task NotifyNextShiftAsync(Notice notice, int? sectorId)
        {
            // Sem setor definido pra quem criou o aviso, não temos como saber
            // qual letra é "o próximo turno" — melhor não notificar do que notificar errado.
            if (sectorId == null) return;

            var now = DateTime.Now;
            var today = now.Date;
            var tomorrow = today.AddDays(1);

            // Restringe a busca às letras do MESMO SETOR de quem criou o aviso
            var sectorLetterIds = await _context.Letters
                .Where(l => l.SectorId == sectorId.Value)
                .Select(l => l.Id)
                .ToListAsync();

            if (sectorLetterIds.Count == 0) return;

            // 1. Busca as escalas de hoje e amanhã, só dentro do setor do autor
            var upcomingSchedules = await _context.ScheduleDays
                .Include(s => s.Shift)
                .Where(s => sectorLetterIds.Contains(s.LetterId)
                         && (s.Date == today || s.Date == tomorrow)
                         && s.Shift.IsDayOff == false)
                .ToListAsync();

            // 2. Monta a linha do tempo real (Junta a Data com a Hora de Início do Turno)
            var nextShiftSchedule = upcomingSchedules
                .Select(s => new
                {
                    Schedule = s,
                    ActualStartTime = s.Date.Add(s.Shift.StartTime)
                })
                .Where(x => x.ActualStartTime > now) // Pega só os turnos que ainda vão começar
                .OrderBy(x => x.ActualStartTime)     // Ordena do mais próximo para o mais distante
                .FirstOrDefault();                   // Pega exatamente o próximo!

            if (nextShiftSchedule == null) return;

            var nextLetterId = nextShiftSchedule.Schedule.LetterId;

            // 3. Busca os usuários que pertencem a essa letra
            var targetUsers = await _context.Users
                .Where(u => u.LetterId == nextLetterId)
                .ToListAsync();

            // 4. Cria a notificação para cada pessoa da próxima equipe
            var notifications = targetUsers.Select(user => new Notification
            {
                TargetUserId = user.Id,
                Message = $"Passagem de plantão: Novo aviso deixado por {notice.CreatedByUser?.UserName ?? "um colega"}.",
                Type = "ShiftHandover",
                ReferenceNoticeId = notice.Id,
                CreatedAt = DateTime.Now,
                IsRead = false
            }).ToList();

            if (notifications.Any())
            {
                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<NoticeCommentResponseDTO> AddCommentAsync(int noticeId, string content, string userId)
        {
            var comment = new NoticeComment
            {
                NoticeId = noticeId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.NoticeComments.Add(comment);
            await _context.SaveChangesAsync();

            // Recarrega o usuário para devolver o nome correto para o React colocar na tela na mesma hora
            var user = await _context.Users.FindAsync(userId);

            return new NoticeCommentResponseDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                CreatedByUserName = user?.UserName ?? "Usuário"
            };
        }

        // ==========================================
        // NOTIFICAÇÕES (sino/campainha do usuário)
        // ==========================================

        public async Task<List<NotificationResponseDTO>> GetNotificationsForUserAsync(string userId, bool onlyUnread = false)
        {
            var query = _context.Notifications.Where(n => n.TargetUserId == userId);

            if (onlyUnread)
                query = query.Where(n => !n.IsRead);

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponseDTO
                {
                    Id = n.Id,
                    Message = n.Message,
                    Type = n.Type,
                    ReferenceNoticeId = n.ReferenceNoticeId,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> MarkNotificationAsReadAsync(int notificationId, string userId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            if (notification.TargetUserId != userId)
                throw new UnauthorizedAccessException("Você não tem permissão para alterar esta notificação.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task MarkAllNotificationsAsReadAsync(string userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.TargetUserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unread)
                notification.IsRead = true;

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // Helper: descobre o Setor de um usuário via a Letra dele
        // ==========================================
        private async Task<int?> GetSectorIdForUserAsync(ApplicationUser? user)
        {
            if (user?.LetterId == null) return null;

            var letter = await _context.Letters.FindAsync(user.LetterId.Value);
            return letter?.SectorId;
        }
    }
}