using FluentValidation;
using Schedule.DTOs.KnowledgeBase.Requests;

namespace Schedule.Validators.KnowledgeBase
{
    /// <summary>
    /// Validador para a requisição de criação de uma nova Categoria.
    /// Garante que os dados de entrada respeitem os limites estruturais antes de atingirem o Service.
    /// </summary>
    public class CreateKnowledgeCategoryRequestValidator : AbstractValidator<CreateKnowledgeCategoryRequest>
    {
        public CreateKnowledgeCategoryRequestValidator()
        {
            // Validações para o Nome da Categoria
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da categoria é obrigatório.")
                .MaximumLength(100).WithMessage("O nome da categoria não pode exceder 100 caracteres.");

            // Validações para a Descrição (Opcional, mas com limite de tamanho)
            RuleFor(x => x.Description)
                .MaximumLength(300).WithMessage("A descrição não pode exceder 300 caracteres.");

            // Nota: O ParentCategoryId é um Guid anulável (Guid?). 
            // O próprio binding do ASP.NET já garante que, se for enviado, tem de ser um formato Guid válido.
            // A validação de negócio (se o ID da categoria pai realmente existe no banco de dados) será feita na camada de Service.
        }
    }
}