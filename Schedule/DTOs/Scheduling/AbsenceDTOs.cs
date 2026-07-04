namespace Schedule.DTOs
{
    public class AbsenceCreateDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string? TargetUserId { get; set; }
    }

    public class AbsenceResponseDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}