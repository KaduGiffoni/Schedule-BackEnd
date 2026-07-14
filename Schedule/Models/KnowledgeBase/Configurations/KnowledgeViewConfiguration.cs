using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeView no SQL Server.
    /// Foca-se na performance de leitura de relatórios e métricas de acesso (RB015).
    /// </summary>
    public class KnowledgeViewConfiguration : IEntityTypeConfiguration<KnowledgeView>
    {
        public void Configure(EntityTypeBuilder<KnowledgeView> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeViews");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.ViewedAt)
                .IsRequired();

            builder.Property(x => x.ArticleId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            // --- Índices de Performance ---

            // Índice no Artigo: Acelera as consultas que calculam quantas vezes um artigo específico foi visto.
            builder.HasIndex(x => x.ArticleId);

            // Índice no Utilizador e na Data: Excelente para relatórios de auditoria 
            // ex: "O que é que o analista João andou a ler na última semana?"
            builder.HasIndex(x => new { x.UserId, x.ViewedAt });

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com o Artigo (N : 1)
            builder.HasOne(x => x.Article)
                .WithMany(a => a.HistoryViews) // Se a coleção foi descomentada na raiz do artigo
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Cascade); // Se o artigo for apagado (Hard Delete), o histórico de views vai junto para não deixar dados órfãos.

            // Relacionamento com o Utilizador (N : 1)
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Se o utilizador for apagado do sistema, o seu histórico de acessos também é removido.
        }
    }
}