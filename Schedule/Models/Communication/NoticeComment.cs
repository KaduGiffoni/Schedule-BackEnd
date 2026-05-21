using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models
{
    public class NoticeComment
    {
        public int Id { get; set; }

        public int NoticeId { get; set; }
        [ForeignKey("NoticeId")]
        public Notice? Notice { get; set; }

        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}