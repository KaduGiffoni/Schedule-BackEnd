using FluentValidation;
using Schedule.DTOs.KnowledgeBase.Requests;
using System.Linq;

namespace Schedule.Validators.KnowledgeBase;

/// <summary>
/// Validador para a requisição de atualização de um Artigo/Procedimento.
/// Garante a integridade dos dados e a obrigatoriedade da justificativa de mudança (RB020).
/// </summary>
public class UpdateKnowledgeArticleRequestValidator : AbstractValidator<UpdateKnowledgeArticleRequest>
{
    public UpdateKnowledgeArticleRequestValidator()
    {
        // Validação do ID raiz (Obrigatório para saber qual artigo atualizar)
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do artigo é obrigatório.");

        // RB008: Título obrigatório. Máximo 150 caracteres.
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título do artigo é obrigatório.")
            .MaximumLength(150).WithMessage("O título não pode exceder 150 caracteres.");

        // RB009: Resumo obrigatório.
        RuleFor(x => x.Summary)
            .NotEmpty().WithMessage("O resumo do artigo é obrigatório.")
            .MaximumLength(500).WithMessage("O resumo não pode exceder 500 caracteres.");

        // Conteúdo principal do procedimento
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("O conteúdo do artigo não pode estar vazio.");

        // RB010: Categoria obrigatória.
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("É obrigatório selecionar uma categoria válida.");

        // RB011: Pelo menos uma Tag obrigatória.
        RuleFor(x => x.TagIds)
            .NotNull().WithMessage("A lista de palavras-chave (Tags) não pode ser nula.")
            .Must(tags => tags.Any()).WithMessage("É obrigatório associar pelo menos uma Tag ao procedimento.");

        // RB023: Tempo estimado e Nível de dificuldade
        RuleFor(x => x.EstimatedTimeInMinutes)
            .GreaterThan(0).WithMessage("O tempo estimado deve ser maior que zero minutos.");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("O nível de dificuldade fornecido não é válido.");

        // Status do artigo (RB012)
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("O status fornecido não é válido.");

        // RB020: Justificativa obrigatória para o histórico (Log de Auditoria / Nova Versão)
        RuleFor(x => x.ChangeDescription)
            .NotEmpty().WithMessage("É obrigatório fornecer uma justificativa/descrição da alteração para gerar a nova versão.")
            .MaximumLength(300).WithMessage("A descrição da alteração não pode exceder 300 caracteres.");
    }
}