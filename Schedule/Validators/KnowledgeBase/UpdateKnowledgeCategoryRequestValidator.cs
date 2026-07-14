using FluentValidation;
using Schedule.DTOs.KnowledgeBase.Requests;

namespace Schedule.Validators.KnowledgeBase
{
    /// <summary>
    /// Validador para a requisição de atualização de uma Categoria existente.
    /// Garante a presença do ID e o respeito aos limites estruturais do banco de dados.
    /// </summary>
    public class UpdateKnowledgeCategoryRequestValidator : AbstractValidator<UpdateKnowledgeCategoryRequest>
    {
        public UpdateKnowledgeCategoryRequestValidator()
        {
            // Validação do ID (Obrigatório para atualizações)
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O identificador da categoria é obrigatório.");

            // Validações para o Nome da Categoria
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da categoria não pode exceder 100 caracteres.");

            // Validações para a Descrição (Opcional, mas com limite)
            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("A descrição não pode exceder 300 caracteres.");

            // O ParentCategoryId (Guid?) é validado implicitamente pelo ASP.NET Core quanto ao formato.
            // A regra de negócio (se o pai existe) ficará no Service.
        }
    }
}