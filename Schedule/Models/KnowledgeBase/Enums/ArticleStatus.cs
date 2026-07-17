namespace Schedule.Models.KnowledgeBase.Enums;

/// <summary>
/// Define os estados de ciclo de vida de um artigo na Base de Conhecimento.
/// </summary>
public enum ArticleStatus
{
    /// <summary>
    /// Rascunho. O artigo está a ser criado ou editado e não deve aparecer nas pesquisas públicas (RB013).
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Publicado. O artigo é considerado um procedimento validado e visível para os leitores.
    /// </summary>
    Published = 2,

    /// <summary>
    /// Arquivado. O procedimento ficou obsoleto ou foi removido da visão principal, mantendo-se acessível apenas para histórico.
    /// </summary>
    Archived = 3
}