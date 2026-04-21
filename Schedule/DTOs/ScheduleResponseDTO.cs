namespace Schedule.DTOs
{
    public class ScheduleResponseDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsDayOff { get; set; }
    }
}