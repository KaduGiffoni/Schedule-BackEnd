using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade de junção KnowledgeArticleReference no SQL Server.
    /// Mapeia o auto-relacionamento Muitos-para-Muitos para citações entre artigos (RB031).
    /// </summary>
    public class KnowledgeArticleReferenceConfiguration : IEntityTypeConfiguration<KnowledgeArticleReference>
    {
        public void Configure(EntityTypeBuilder<KnowledgeArticleReference> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeArticleReferences");

            // --- Chave Primária Composta ---
            // A união do Artigo Origem com o Artigo Destino forma a identificação única deste registo.
            // Impede que um artigo referencie o mesmo pré-requisito duas vezes.
            builder.HasKey(x => new { x.ArticleId, x.ReferencedArticleId });

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com o Artigo de Origem (quem está a fazer a citação)
            builder.HasOne(x => x.Article)
                .WithMany() // Poderá alterar para .WithMany(a => a.References) se a coleção estiver exposta
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Restrict); // Previne o erro "Multiple Cascade Paths" do SQL Server.

            // Relacionamento com o Artigo Referenciado (o alvo da citação)
            builder.HasOne(x => x.ReferencedArticle)
                .WithMany()
                .HasForeignKey(x => x.ReferencedArticleId)
                .OnDelete(DeleteBehavior.Restrict); // Protege o artigo referenciado de ser apagado se alguém o estiver a citar.
        }
    }
}