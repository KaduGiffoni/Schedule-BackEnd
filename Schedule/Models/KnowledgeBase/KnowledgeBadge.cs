using System;
using System.Collections.Generic;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que representa uma Conquista (Badge/Selo) no sistema de Gamificação.
    /// Atende à regra RB033 (Usuários ganham Conquistas ao lerem todos os artigos de uma categoria).
    /// </summary>
    public class KnowledgeBadge
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Nome da conquista (ex: "Especialista em CUCM" ou "Mestre do Teams Phone").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do que significa este selo (aparece ao passar o mouse no perfil).
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Caminho ou URL para a imagem (PNG/SVG) que representa o selo visualmente.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Data em que este selo foi cadastrado no sistema.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Chaves Estrangeiras ---

        /// <summary>
        /// ID da categoria (KnowledgeCategory) à qual este selo pertence. 
        /// O sistema avaliará os artigos desta categoria para conceder ou bloquear a conquista.
        /// </summary>
        public Guid CategoryId { get; set; }

        // --- Propriedades de Navegação ---

        public virtual KnowledgeCategory? Category { get; set; }

        /* * Nota de Arquitetura:
         * A lista que liga este selo aos usuários que o conquistaram será criada a seguir.
         * Deixo comentado para manter a compilação do projeto ilesa neste exato momento.
         */
        // public virtual ICollection<UserKnowledgeBadge> UserBadges { get; set; } = new List<UserKnowledgeBadge>();
    }
}