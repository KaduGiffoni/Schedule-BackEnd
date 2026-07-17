namespace Schedule.DTOs.KnowledgeBase.Requests
{
    public record CreateKnowledgeTagRequest
    {
        public string Name { get; init; } = string.Empty;
    }
}