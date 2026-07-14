using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade UserKnowledgeBadge no SQL Server.
    /// Mapeia o progresso do sistema de Gamificação e a regra do selo inativo (RB034).
    /// </summary>
    public class UserKnowledgeBadgeConfiguration : IEntityTypeConfiguration<UserKnowledgeBadge>
    {
        public void Configure(EntityTypeBuilder<UserKnowledgeBadge> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("UserKnowledgeBadges");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.EarnedAt)
                .IsRequired();

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true); // Por defeito, a conquista nasce ativa (colorida)

            builder.Property(x => x.BadgeId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            // --- Índices de Performance e Unicidade ---

            // CRÍTICO: Impede que o sistema insira o mesmo troféu duplicado para o mesmo utilizador.
            // A atualização de status (RB034) ocorrerá sempre sobre esta linha única.
            builder.HasIndex(x => new { x.UserId, x.BadgeId })
                .IsUnique();

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com o Selo/Troféu (N : 1)
            builder.HasOne(x => x.Badge)
                .WithMany(b => b.UserBadges) // Se a coleção foi descomentada em KnowledgeBadge
                .HasForeignKey(x => x.BadgeId)
                .OnDelete(DeleteBehavior.Cascade); // Se a gestão apagar o selo do sistema, o progresso dos utilizadores nesse selo também desaparece.

            // Relacionamento com o Utilizador (N : 1)
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Se o analista for desligado e removido do sistema, os seus selos são eliminados para poupar espaço.
        }
    }
}