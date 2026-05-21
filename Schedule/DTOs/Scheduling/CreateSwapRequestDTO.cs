namespace Schedule.DTOs
{
    public class CreateSwapRequestDTO
    {

        public string TargetUserId { get; set; } = string.Empty;
        public int ScheduleDayId { get; set; }

    }
}
