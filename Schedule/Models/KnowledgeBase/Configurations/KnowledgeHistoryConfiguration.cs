using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations;

/// <summary>
/// Configuração Fluente (Fluent API) para a entidade KnowledgeHistory no SQL Server.
/// Garante a persistência segura e a indexação do Log de Auditoria do módulo (RB020).
/// </summary>
public class KnowledgeHistoryConfiguration : IEntityTypeConfiguration<KnowledgeHistory>
{
    public void Configure(EntityTypeBuilder<KnowledgeHistory> builder)
    {
        // Nome da Tabela na Base de Dados (Pluralizado corretamente)
        builder.ToTable("KnowledgeHistories");

        // Chave Primária
        builder.HasKey(x => x.Id);

        // Propriedades e Restrições
        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(100); // Ex: "StatusChanged", "SoftDeleted", "NewVersion"

        builder.Property(x => x.Details)
            .HasMaxLength(1000); // Detalhes amigáveis sobre o evento

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.ArticleId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        // --- Índices de Performance ---

        // Índice para carregar rapidamente toda a linha do tempo (timeline) de eventos de um artigo específico
        builder.HasIndex(x => x.ArticleId);

        // Índice composto para auditoria focada no utilizador e no tempo
        // ex: "Quais as ações que o administrador realizou hoje de manhã?"
        builder.HasIndex(x => new { x.UserId, x.CreatedAt });

        // --- Relacionamentos (Chaves Estrangeiras) ---

        // Relacionamento com o Artigo (N : 1)
        builder.HasOne(x => x.Article)
            .WithMany()
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade); // Se o artigo raiz for destruído fisicamente, o log associado é removido.

        // Relacionamento com o Utilizador que gerou a ação (N : 1)
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict); // CRÍTICO: Se um utilizador for apagado, o histórico do que ele fez na KB permanece por questões de auditoria.
    }
}