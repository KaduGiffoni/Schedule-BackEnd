namespace Schedule.DTOs
{
    public class UpdateProfileDTO
    {
        public string Email { get; set; } = string.Empty;
        public int? LetterId { get; set; }
        public string CompleteName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Registration { get; set; } = string.Empty;
    }
}