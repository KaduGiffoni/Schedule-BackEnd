using System;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que unifica o armazenamento de Imagens, Links de Vídeos e Documentos Anexos.
    /// Garante que as mídias pertençam a uma versão específica do artigo (RB017)
    /// e que vídeos sejam apenas referenciados por URL (RB018, RB019).
    /// </summary>
    public class KnowledgeMedia
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Nome de exibição original do arquivo (ex: "topologia-switch.png" ou "Tutorial em Vídeo").
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Caminho físico no servidor/Storage (para imagens e documentos) 
        /// ou a URL externa completa (para vídeos no YouTube, SharePoint, Stream).
        /// Nunca armazenaremos o binário do vídeo no banco (RB018).
        /// </summary>
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// Define como o frontend deve renderizar esta mídia (tag img, iframe ou link de download).
        /// </summary>
        public MediaType Type { get; set; } = MediaType.Image;

        /// <summary>
        /// Data em que a mídia foi anexada ou o link inserido.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID da versão (KnowledgeArticleVersion) a qual esta mídia pertence.
        /// A amarração é feita na versão para garantir a imutabilidade do histórico (RB017).
        /// </summary>
        public Guid ArticleVersionId { get; set; }

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticleVersion? ArticleVersion { get; set; }
    }
}