namespace Schedule.DTOs
{
    // 1. O que o React ENVIA para o C# ao criar um aviso
    public class NoticeCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Geral" ou "Turno"
    }

    // 2. O que o C# DEVOLVE para o React desenhar os cards na tela
    public class NoticeResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;

        // Lista de comentários amarrados a esse aviso
        public List<NoticeCommentResponseDTO> Comments { get; set; } = new();
    }

    // 3. O que o React ENVIA para adicionar um comentário na Passagem de Turno
    public class NoticeCommentCreateDTO
    {
        public string Content { get; set; } = string.Empty;
    }

    // 4. O que o C# DEVOLVE sobre o Comentário
    public class NoticeCommentResponseDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
    }
}