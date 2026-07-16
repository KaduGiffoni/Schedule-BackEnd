using System;
using Schedule.Models;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que armazena a fotografia do conteúdo de um artigo em um determinado momento.
    /// Toda edição gera um novo registro aqui, garantindo histórico e rastreabilidade (RB004, RB005).
    /// </summary>
    public class KnowledgeArticleVersion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Número sequencial da versão (Ex: 1, 2, 3...).
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// Título do artigo nesta versão. Máximo de 150 caracteres (RB008).
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Resumo obrigatório que aparece nos cards de pesquisa (RB009).
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo completo do procedimento (Markdown ou HTML enriquecido).
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Nível de dificuldade definido pelo autor nesta versão (RB023).
        /// </summary>
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Basic;

        /// <summary>
        /// Tempo estimado de leitura ou execução em minutos (RB023).
        /// </summary>
        public int EstimatedTimeInMinutes { get; set; }

        /// <summary>
        /// Motivo da edição/alteração. Essencial para auditoria (RB020).
        /// Ex: "Atualizado o comando do switch modelo X".
        /// </summary>
        public string? ChangeDescription { get; set; }

        /// <summary>
        /// Data e hora exatas em que esta versão foi criada/salva.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID do artigo raiz ao qual esta versão pertence.
        /// </summary>
        public Guid ArticleId { get; set; }

        /// <summary>
        /// Usuário que realizou a edição/criação desta versão específica (RB020).
        /// </summary>
        public string EditorId { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        public virtual KnowledgeArticle? Article { get; set; }
        public virtual ApplicationUser? Editor { get; set; }

        /* * Nota de Arquitetura:
         * A lista de mídias (Imagens, Documentos, Links de Vídeo - RB017) 
         * pertencerá à Versão e não ao Artigo raiz. Isso garante que, se alguém apagar
         * uma imagem em uma edição, a versão antiga do artigo continuará exibindo a imagem antiga.
         * Será descomentado quando criarmos a entidade KnowledgeMedia.
         */
        // public virtual ICollection<KnowledgeMedia> Media { get; set; } = new List<KnowledgeMedia>();
    }
}