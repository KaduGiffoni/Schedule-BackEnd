using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeArticle no SQL Server.
    /// </summary>
    public class KnowledgeArticleConfiguration : IEntityTypeConfiguration<KnowledgeArticle>
    {
        public void Configure(EntityTypeBuilder<KnowledgeArticle> builder)
        {
            // Nome da Tabela no Banco de Dados
            builder.ToTable("KnowledgeArticles");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(180); // Um pouco maior que o título para acomodar sufixos se necessário

            builder.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>(); // Salva o Enum como INT no banco de dados

            builder.Property(x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.ViewCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.FavoriteCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.AuthorId)
                .IsRequired();

            // Índices de Performance e Unicidade
            builder.HasIndex(x => x.Slug)
                .IsUnique(); // Garante a regra RB007 (Slug deve ser único)

            builder.HasIndex(x => x.IsDeleted); // Índice para acelerar a filtragem do Soft Delete
            builder.HasIndex(x => x.Status);    // Índice para acelerar as buscas públicas (RB013)

            // --- Relacionamentos (Mapeamento de Chaves Estrangeiras) ---

            // Relacionamento com Categoria (1 : N)
            builder.HasOne(x => x.Category)
                .WithMany() // Se preferir expor uma lista na Categoria futuramente, altere aqui
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Impede que apagar uma categoria delete os artigos (Segurança)

            // Relacionamento com o Autor original (1 : N)
            builder.HasOne(x => x.Author)
                .WithMany()
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); // Se o usuário for deletado, o artigo histórico permanece

            // Relacionamento com a Versão Atual (Ponteiro de Otimização 1 : 1)
            builder.HasOne<KnowledgeArticleVersion>()
                .WithMany()
                .HasForeignKey(x => x.CurrentVersionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}