using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models.Communication
{
    public class NoticeAcknowledgment
    {
        public int Id { get; set; }

        public int NoticeId { get; set; }
        [ForeignKey("NoticeId")]
        public Notice? Notice { get; set; }

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public DateTime AcknowledgedAt { get; set; } = DateTime.Now;
    }
}