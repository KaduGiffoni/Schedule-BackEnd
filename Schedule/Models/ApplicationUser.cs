using Microsoft.AspNetCore.Identity;

namespace Schedule.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string CompleteName { get; set; } = string.Empty;
        public string Registration { get; set; } = string.Empty;

        public int? LetterId { get; set; }
        public Letter? Letter { get; set; }
    }
}
