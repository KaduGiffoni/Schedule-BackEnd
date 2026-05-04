namespace Schedule.Models
{
    public class ShiftPattern
    {
        public int Id { get; set; }

        // O nome descreve o RITMO, não o setor. Ex: "Escala 8h Francesa", "Escala 12h (2x2x4)", "Administrativo (5x2)"
        public string Name { get; set; }

        // A matemática pura: "4,2,2,2,2,4,4,3,3,3,3,4,4,1,1,1,1,4"
        public string Sequence { get; set; }
    }
}