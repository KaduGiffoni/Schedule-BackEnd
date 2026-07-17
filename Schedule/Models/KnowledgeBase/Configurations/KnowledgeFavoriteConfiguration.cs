using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

/// <summary>
/// Configuração Fluente (Fluent API) para a entidade KnowledgeFavorite no SQL Server.
/// Garante a integridade dos favoritos e impede duplicações por utilizador (RB014).
/// </summary>
public class KnowledgeFavoriteConfiguration : IEntityTypeConfiguration<KnowledgeFavorite>
{
    public void Configure(EntityTypeBuilder<KnowledgeFavorite> builder)
    {
        // Nome da Tabela na Base de Dados
        builder.ToTable("KnowledgeFavorites");

        // Chave Primária
        builder.HasKey(x => x.Id);

        // Propriedades e Restrições
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        // --- Índices de Performance e Unicidade ---

        // Impede que o mesmo utilizador favorite o mesmo artigo mais de uma vez.
        // A união de ArticleId + UserId deve ser sempre única.
        builder.HasIndex(x => new { x.ArticleId, x.UserId })
            .IsUnique();

        // --- Relacionamentos (Chaves Estrangeiras) ---

        // Relacionamento com o Artigo (N : 1)
        builder.HasOne(x => x.Article)
            .WithMany(a => a.Favorites)
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade); // Se o artigo for apagado fisicamente, os favoritos desaparecem.

        // Relacionamento com o Utilizador (N : 1)
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Se o utilizador for apagado, os seus favoritos vão junto.
    }
}