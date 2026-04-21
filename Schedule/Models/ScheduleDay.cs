using System.Text.Json.Serialization;

namespace Schedule.Models
{
    public class ScheduleDay
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int LetterId { get; set; }
        [JsonIgnore]
        public Letter? Letter { get; set; }

        public int ShiftId { get; set; }
        [JsonIgnore]
        public Shift? Shift { get; set; }

    }
}
