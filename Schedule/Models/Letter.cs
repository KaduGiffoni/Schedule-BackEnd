using System.Text.Json.Serialization;

namespace Schedule.Models
{
    public class Letter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SectorId { get; set; }
        public Sector? Sector { get; set; }

        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
