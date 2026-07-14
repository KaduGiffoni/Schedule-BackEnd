using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeCategory no SQL Server.
    /// </summary>
    public class KnowledgeCategoryConfiguration : IEntityTypeConfiguration<KnowledgeCategory>
    {
        public void Configure(EntityTypeBuilder<KnowledgeCategory> builder)
        {
            // Nome da Tabela no Banco de Dados
            builder.ToTable("KnowledgeCategories");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(x => x.Description)
                .HasMaxLength(300);

            // --- Índices de Performance e Unicidade ---

            // Garante que não existam slugs de categorias repetidos para evitar colisões de rotas
            builder.HasIndex(x => x.Slug)
                .IsUnique();

            // --- Relacionamentos (Auto-relacionamento para Hierarquia de Subcategorias) ---

            // Uma Categoria (Subcategoria) possui uma Categoria Pai (N : 1) (RB021)
            builder.HasOne(x => x.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(x => x.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Segurança: Impede apagar uma categoria pai que contenha subcategorias vinculadas
        }
    }
}