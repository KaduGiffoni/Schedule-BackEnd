using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeArticleRead no SQL Server.
    /// Garante a integridade do controlo de leitura e impede recibos duplicados (RB032).
    /// </summary>
    public class KnowledgeArticleReadConfiguration : IEntityTypeConfiguration<KnowledgeArticleRead>
    {
        public void Configure(EntityTypeBuilder<KnowledgeArticleRead> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeArticleReads");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.ReadAt)
                .IsRequired();

            builder.Property(x => x.ArticleId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            // --- Índices de Performance e Unicidade ---

            // CRÍTICO: Impede que o mesmo utilizador marque o mesmo artigo como lido mais de uma vez.
            // A combinação de Artigo + Utilizador tem de ser estritamente única.
            builder.HasIndex(x => new { x.ArticleId, x.UserId })
                .IsUnique();

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com o Artigo Raiz (N : 1)
            builder.HasOne(x => x.Article)
                .WithMany(a => a.ReadReceipts) // Se a coleção foi descomentada na entidade raiz
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Cascade); // Se um artigo for destruído (Hard Delete), o seu histórico de leitura é limpo.

            // Relacionamento com o Utilizador (N : 1)
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Se o funcionário for apagado do sistema, limpamos o seu progresso de leitura para libertar espaço.
        }
    }
}