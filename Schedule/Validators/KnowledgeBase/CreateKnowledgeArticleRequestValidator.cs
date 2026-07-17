using FluentValidation;
using Schedule.DTOs.KnowledgeBase.Requests;
using System.Linq;

namespace Schedule.Validators.KnowledgeBase;

/// <summary>
/// Validador para a requisição de criação de um novo Artigo/Procedimento.
/// Aplica as regras de negócio estruturais (RB008, RB009, RB010, RB011, RB023).
/// </summary>
public class CreateKnowledgeArticleRequestValidator : AbstractValidator<CreateKnowledgeArticleRequest>
{
    public CreateKnowledgeArticleRequestValidator()
    {
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
    }
}