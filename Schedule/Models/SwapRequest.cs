using System.ComponentModel.DataAnnotations;

namespace Schedule.Models
{
    public enum RequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
    }
    public class SwapRequest
    {
            public int Id { get; set; }

            [Required]
            public string RequestingUserId { get; set; } = string.Empty;
            public ApplicationUser? RequestingUser { get; set; }

           
            [Required]
            public string TargetUserId { get; set; } = string.Empty;
            public ApplicationUser? TargetUser { get; set; }

            
            public int ScheduleDayId { get; set; }
            public ScheduleDay? ScheduleDay { get; set; }

            public RequestStatus Status { get; set; } = RequestStatus.Pending;
            public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
