using System.Text.Json.Serialization;

namespace Schedule.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOutsource { get; set; }

        public ICollection<Sector> Sectors { get; set; } = new List<Sector>();
    }
}
