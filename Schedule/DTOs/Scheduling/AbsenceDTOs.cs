using Schedule.Models;
using System.ComponentModel.DataAnnotations;

namespace Schedule.DTOs
{
    public class AbsenceCreateDTO
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        // Ferias, CompensacaoHora, Atestado, Falta, Outro
        public AbsenceType Type { get; set; } = AbsenceType.Vacation;

        // Se vier vazio, assume o usuário logado (auto-lançamento)
        public string? TargetUserId { get; set; }

        // Quem vai cobrir enquanto a pessoa estiver ausente (opcional)
        public string? SubstituteUserId { get; set; }

        public string? Notes { get; set; }
    }

    public class AbsenceResponseDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public AbsenceType Type { get; set; }
        public string TypeDescription { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }

        public string? SubstituteUserId { get; set; }
        public string? SubstituteUserName { get; set; }

        public string? Notes { get; set; }
    }

    // Usado para "sobrepor" a informação de ausência em cima de um dia da escala (calendário)
    public class AbsenceOverlayDTO
    {
        public int AbsenceId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public AbsenceType Type { get; set; }
        public string TypeDescription { get; set; } = string.Empty;

        public string? SubstituteUserId { get; set; }
        public string? SubstituteUserName { get; set; }
    }
}