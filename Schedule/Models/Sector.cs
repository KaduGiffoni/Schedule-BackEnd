using System.Text.Json.Serialization;

namespace Schedule.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int CompanyId { get; set; }
        public Company? Company { get; set; }

        public ICollection<Letter> Letters { get; set; } = new List<Letter>();
    }
}
