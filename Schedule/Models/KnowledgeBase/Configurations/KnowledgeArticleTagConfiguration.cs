using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade de junção KnowledgeArticleTag no SQL Server.
    /// Mapeia o relacionamento Muitos-para-Muitos e garante a unicidade da ligação.
    /// </summary>
    public class KnowledgeArticleTagConfiguration : IEntityTypeConfiguration<KnowledgeArticleTag>
    {
        public void Configure(EntityTypeBuilder<KnowledgeArticleTag> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeArticleTags");

            // --- Chave Primária Composta ---
            // A união do ID do Artigo com o ID da Tag forma a identificação única deste registo.
            builder.HasKey(x => new { x.ArticleId, x.TagId });

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com o Artigo (N : 1)
            builder.HasOne(x => x.Article)
                .WithMany() // Pode ser substituído por .WithMany(a => a.ArticleTags) se a coleção estiver ativa na entidade raiz
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Cascade); // Se o artigo for apagado (Hard Delete), limpamos as tags vinculadas a ele.

            // Relacionamento com a Tag (N : 1)
            builder.HasOne(x => x.Tag)
                .WithMany() // Pode ser substituído por .WithMany(t => t.ArticleTags)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade); // Se um Admin apagar uma Tag do sistema, removemos o vínculo dela em todos os artigos.
        }
    }
}