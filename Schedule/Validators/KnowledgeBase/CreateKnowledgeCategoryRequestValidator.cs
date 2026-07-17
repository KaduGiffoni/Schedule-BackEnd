using FluentValidation;
using Schedule.DTOs.KnowledgeBase.Requests;

namespace Schedule.Validators.KnowledgeBase;

/// <summary>
/// Validador para a requisição de criação de uma nova Categoria.
/// Garante que os dados de entrada respeitem os limites estruturais antes de atingirem o Service.
/// </summary>
public class CreateKnowledgeCategoryRequestValidator : AbstractValidator<CreateKnowledgeCategoryRequest>
{
    public CreateKnowledgeCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria não pode exceder 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("A descrição não pode exceder 300 caracteres.");
    }
}