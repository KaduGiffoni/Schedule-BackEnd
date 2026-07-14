using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeComment no SQL Server.
    /// Define limites de texto e protege a base de conhecimento corporativa.
    /// </summary>
    public class KnowledgeCommentConfiguration : IEntityTypeConfiguration<KnowledgeComment>
    {
        public void Configure(EntityTypeBuilder<KnowledgeComment> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeComments");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(1500); // Limite generoso, mas impede que colem textos infinitos no campo de discussão

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ArticleId)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            // --- Índices de Performance ---

            // Índice composto para carregar rapidamente os comentários de um artigo ordenados por data
            builder.HasIndex(x => new { x.ArticleId, x.CreatedAt });

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com o Artigo (N : 1)
            builder.HasOne(x => x.Article)
                .WithMany(a => a.Comments) // Se descomentou a coleção na entidade KnowledgeArticle
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Cascade); // Se o artigo raiz for apagado (Hard Delete), apagamos toda a discussão.

            // Relacionamento com o Utilizador (N : 1)
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Diferente dos Favoritos, AQUI usamos Restrict. Se o analista sair da empresa, os comentários técnicos dele não podem desaparecer.
        }
    }
}