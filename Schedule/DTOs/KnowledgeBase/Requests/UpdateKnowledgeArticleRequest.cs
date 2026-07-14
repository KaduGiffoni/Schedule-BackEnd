using System;
using System.Collections.Generic;
using Schedule.Models.KnowledgeBase.Enums;

namespace Schedule.DTOs.KnowledgeBase.Requests
{
    /// <summary>
    /// Objeto de transferência de dados (DTO) para a requisição de atualização de um Artigo.
    /// Os dados fornecidos aqui gerarão uma nova versão do documento (RB004).
    /// </summary>
    public class UpdateKnowledgeArticleRequest
    {
        /// <summary>
        /// ID raiz do artigo que está a ser atualizado. (Obrigatório)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Novo título do artigo (caso tenha sofrido alteração).
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Novo resumo do artigo.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Conteúdo detalhado atualizado (Markdown/HTML).
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Novo tempo estimado de leitura/execução em minutos.
        /// </summary>
        public int EstimatedTimeInMinutes { get; set; }

        /// <summary>
        /// Nível de complexidade atualizado do procedimento.
        /// </summary>
        public DifficultyLevel Difficulty { get; set; }

        /// <summary>
        /// ID da Categoria. Permite realocar o artigo para outra área.
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Status do artigo (ex: arquivar, manter publicado, voltar para rascunho).
        /// </summary>
        public ArticleStatus Status { get; set; }

        /// <summary>
        /// Lista de IDs das Tags (sobrescreve a lista anterior).
        /// </summary>
        public IEnumerable<Guid> TagIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Lista de IDs de artigos referenciados atualizada.
        /// </summary>
        public IEnumerable<Guid> ReferencedArticleIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Descrição curta do que foi alterado nesta versão. 
        /// Fundamental para o log de auditoria (RB020).
        /// </summary>
        public string ChangeDescription { get; set; } = string.Empty;
    }
}