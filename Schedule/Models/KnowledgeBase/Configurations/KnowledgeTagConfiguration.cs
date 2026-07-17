using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

/// <summary>
/// Configuração Fluente (Fluent API) para a entidade KnowledgeTag no SQL Server.
/// Garante as restrições de tamanho e a unicidade das palavras-chave (RB022).
/// </summary>
public class KnowledgeTagConfiguration : IEntityTypeConfiguration<KnowledgeTag>
{
    public void Configure(EntityTypeBuilder<KnowledgeTag> builder)
    {
        // Nome da Tabela na Base de Dados
        builder.ToTable("KnowledgeTags");

        // Chave Primária
        builder.HasKey(x => x.Id);

        // Propriedades e Restrições
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50); // Tags geralmente são palavras ou expressões curtas

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(60);

        // --- Índices de Performance e Unicidade ---

        // Garante a regra de negócio RB022 (Tags não podem duplicar)
        // O Slug normalizado (minúsculas, sem espaços) será a base para esta restrição.
        builder.HasIndex(x => x.Slug)
            .IsUnique();
    }
}