using FluentValidation;
using Schedule.DTOs.KnowledgeBase.Requests;

namespace Schedule.Validators.KnowledgeBase;

/// <summary>
/// Validador para a requisição de atualização de uma Categoria existente.
/// Garante a presença do ID e o respeito aos limites estruturais do banco de dados.
/// </summary>
public class UpdateKnowledgeCategoryRequestValidator : AbstractValidator<UpdateKnowledgeCategoryRequest>
{
    public UpdateKnowledgeCategoryRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da categoria não pode exceder 100 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("A descrição não pode exceder 300 caracteres.");
    }
}