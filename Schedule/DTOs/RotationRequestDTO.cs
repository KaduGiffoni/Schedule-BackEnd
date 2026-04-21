namespace Schedule.DTOs
{
    public class RotationRequestDTO
    {
        public int LetterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<int> ShiftRotationIds { get; set; } = new List<int>();
    }
}
