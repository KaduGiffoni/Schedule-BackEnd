namespace Schedule.Models.KnowledgeBase.Enums;

/// <summary>
/// Define os tipos de multimédia que podem ser anexados a uma versão de um artigo na Base de Conhecimento.
/// </summary>
public enum MediaType
{
    /// <summary>
    /// Ficheiro de imagem físico armazenado no servidor/nuvem (ex: PNG, JPG).
    /// Pertence à versão do artigo (RB017).
    /// </summary>
    Image = 1,

    /// <summary>
    /// Apenas a hiperligação (URL) apontando para plataformas externas como YouTube, SharePoint ou Microsoft Stream.
    /// Conforme a regra de negócio, os vídeos não são armazenados fisicamente na base de dados (RB018, RB019).
    /// </summary>
    VideoLink = 2,

    /// <summary>
    /// Documentos diversos (ex: PDFs, Scripts, DOCX) que complementam o procedimento operacional.
    /// </summary>
    Document = 3
}