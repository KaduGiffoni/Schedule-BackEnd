namespace Schedule.DTOs
{
    public class HolidayCreateDTO
    {
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.Now;

        public string type { get; set; } = "Customizado";
        public bool IsRecurring { get; set; } = false;

    }

    public class HolidayUpdateDTO
    {
        public string Name { get; set; } = string.Empty;

        public DateTime Date { get; set; } 

        public string Type { get; set; } = string.Empty;
        public bool IsRecurring { get; set; } = false;

    }
}
