namespace Schedule.DTOs
{
    public class RotationRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Agora o gatilho é o Setor Inteiro!
        public int SectorId { get; set; }
        public int ShiftPatternId { get; set; }
    }
}