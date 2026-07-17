using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models.Scheduling
{
    public enum AbsenceType
    {
        Vacation = 0,
        CompensatoryTime = 1,
        MedicalCertificate = 2,
        Absence = 3,
        Other = 4
    }

    public static class AbsenceTypeExtensions
    {
        // Usado pra montar um texto amigável pro front (ex: dropdown, badge no calendário)
        public static string ToDisplayName(this AbsenceType type) => type switch
        {
            AbsenceType.Vacation => "Férias",
            AbsenceType.CompensatoryTime => "Compensação de Hora",
            AbsenceType.MedicalCertificate => "Atestado",
            AbsenceType.Absence => "Falta",
            AbsenceType.Other => "Outro",
            _ => type.ToString()
        };
    }

    public class UserAbsence
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        // Tipo da ausência: férias, compensação de hora, atestado, falta, outro
        public AbsenceType Type { get; set; } = AbsenceType.Vacation;

        // Dia exato ou primeiro dia das férias
        public DateTime StartDate { get; set; }

        // Mesmo dia do StartDate (se for 1 dia só, ex: compensação de hora) ou o último dia das férias
        public DateTime EndDate { get; set; }

        // Quem vai cobrir o posto/turno enquanto o usuário estiver ausente (opcional)
        public string? SubstituteUserId { get; set; }
        [ForeignKey("SubstituteUserId")]
        public ApplicationUser? SubstituteUser { get; set; }

        // Observação livre (ex: "Compensar hora extra do plantão de 20/08")
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public int TotalDays => (EndDate.Date - StartDate.Date).Days + 1;
    }
}