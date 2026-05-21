using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string TargetUserId { get; set; } = string.Empty;
        [ForeignKey("TargetUserId")]
        public ApplicationUser? TargetUser { get; set; }

        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int? ReferenceNoticeId { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}