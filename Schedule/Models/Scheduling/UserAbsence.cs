using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models
{
    public class UserAbsence
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        // Dia exato ou primeiro dia das férias
        public DateTime StartDate { get; set; }

        // Mesmo dia do StartDate (se for 1 dia só) ou o último dia das férias
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}