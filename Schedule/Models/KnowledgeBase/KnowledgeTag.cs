using System;
using System.Collections.Generic;

namespace Schedule.Models.KnowledgeBase
{
    /// <summary>
    /// Entidade que representa uma Tag (palavra-chave) para categorização, cruzamento e pesquisa rápida de artigos.
    /// Atende às regras de busca (RB026), artigos relacionados (RB030) e não duplicidade (RB022).
    /// </summary>
    public class KnowledgeTag
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Nome de exibição da tag (ex: "Cisco", "VLAN", "Firewall").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Identificador padronizado e único (ex: "cisco", "vlan", "firewall"). 
        /// Será usado para garantir que não haja tags duplicadas no banco (RB022) através de um Unique Index.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        // --- Propriedades de Navegação ---

        /* * Nota de Arquitetura:
         * A coleção que liga as Tags aos Artigos usará uma tabela de junção explícita (KnowledgeArticleTag).
         * Deixarei comentado por enquanto para garantir a compilação até gerarmos a entidade correspondente no próximo passo.
         */
        // public virtual ICollection<KnowledgeArticleTag> ArticleTags { get; set; } = new List<KnowledgeArticleTag>();
    }
}