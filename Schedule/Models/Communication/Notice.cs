using System.ComponentModel.DataAnnotations.Schema;

namespace Schedule.Models
{
    public class Notice
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = "Ativo";
        public string CreatedByUserId { get; set; } = string.Empty;
        [ForeignKey("CreatedByUserId")]
        public ApplicationUser? CreatedByUser { get; set; }
        public List<NoticeAcknowledgment> Acknowledgments { get; set; } = new();
        public List<NoticeComment> Comments { get; set; } = new();
    }
}