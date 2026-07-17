namespace Schedule.Models.Scheduling
{
    public class Letter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ex: "A", "B", "C", "D"

        public int SectorId { get; set; }
        public Sector? Sector { get; set; }
        public int PatternOffset { get; set; } = 0;
    }
}