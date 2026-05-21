namespace Schedule.Models
{
    public class Holiday
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Type { get; set; } = "Nacional";
        public bool IsHoliday { get; set; } = true;
        public bool IsRecurring { get; set; } = false;
    }
}
