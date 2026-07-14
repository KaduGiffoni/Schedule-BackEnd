namespace Schedule.Models.KnowledgeBase.Enums
{
    /// <summary>
    /// Define o nível de dificuldade estimado para compreensão ou execução do procedimento descrito no artigo.
    /// </summary>
    public enum DifficultyLevel
    {
        /// <summary>
        /// Procedimento básico, adequado para analistas de nível júnior ou operações rotineiras simples.
        /// </summary>
        Basic = 1,

        /// <summary>
        /// Procedimento intermediário, requer conhecimento prévio moderado sobre o assunto ou equipamento.
        /// </summary>
        Intermediate = 2,

        /// <summary>
        /// Procedimento avançado, destinado a especialistas ou analistas seniores. Pode envolver configurações complexas ou riscos críticos.
        /// </summary>
        Advanced = 3
    }
}