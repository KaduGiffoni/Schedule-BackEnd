using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeArticleVersion no SQL Server.
    /// </summary>
    public class KnowledgeArticleVersionConfiguration : IEntityTypeConfiguration<KnowledgeArticleVersion>
    {
        public void Configure(EntityTypeBuilder<KnowledgeArticleVersion> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeArticleVersions");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.VersionNumber)
                .IsRequired();

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150); // Regra RB008: Título obrigatório e com máximo de 150 caracteres.

            builder.Property(x => x.Summary)
                .IsRequired()
                .HasMaxLength(500); // Regra RB009: Resumo obrigatório.

            // O campo Content conterá muito texto (Markdown/HTML), por isso não tem MaxLength.
            // O Entity Framework mapeará isto nativamente para nvarchar(max) no SQL Server.
            builder.Property(x => x.Content)
                .IsRequired();

            builder.Property(x => x.Difficulty)
                .IsRequired()
                .HasConversion<int>(); // Grava o Enum como um número inteiro na base de dados.

            builder.Property(x => x.EstimatedTimeInMinutes)
                .IsRequired();

            builder.Property(x => x.ChangeDescription)
                .HasMaxLength(500); // Descrição da alteração opcional, mas com limite para não abusarem (RB020).

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.EditorId)
                .IsRequired();

            // --- Índices de Performance ---

            // Índice para acelerar drasticamente a procura por todas as versões de um determinado artigo.
            builder.HasIndex(x => x.ArticleId);

            // --- Relacionamentos (Mapeamento de Chaves Estrangeiras) ---

            // Relacionamento com o Artigo Raiz (N : 1)
            builder.HasOne(x => x.Article)
                .WithMany(a => a.Versions)
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Cascade); // Se um dia o artigo for apagado fisicamente (Hard Delete), as suas versões são apagadas em cascata.

            // Relacionamento com o Editor (N : 1)
            builder.HasOne(x => x.Editor)
                .WithMany()
                .HasForeignKey(x => x.EditorId)
                .OnDelete(DeleteBehavior.Restrict); // Impede que o sistema apague a versão caso o utilizador seja apagado da empresa (mantém histórico).
        }
    }
}