using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeBadge no SQL Server.
    /// Define a estrutura dos Troféus/Selos do sistema de Gamificação (RB033).
    /// </summary>
    public class KnowledgeBadgeConfiguration : IEntityTypeConfiguration<KnowledgeBadge>
    {
        public void Configure(EntityTypeBuilder<KnowledgeBadge> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeBadges");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100); // Ex: "Especialista em CUCM"

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500); // Descrição curta para aparecer numa tooltip no perfil do utilizador

            builder.Property(x => x.ImageUrl)
                .IsRequired()
                .HasMaxLength(2048); // Previne erros caso a imagem esteja alojada num storage com URL muito longa

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.CategoryId)
                .IsRequired();

            // --- Índices de Performance ---

            // Índice para encontrar rapidamente qual o selo associado a uma determinada categoria
            builder.HasIndex(x => x.CategoryId);

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com a Categoria (N : 1)
            // Um selo pertence a uma categoria específica.
            builder.HasOne(x => x.Category)
                .WithMany(c => c.Badges) // Se descomentou a coleção na entidade KnowledgeCategory
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // Se o Administrador apagar fisicamente a categoria "CUCM", o troféu associado a ela também é removido.
        }
    }
}