namespace Schedule.DTOs.Core
{
    public class AdminResetPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
