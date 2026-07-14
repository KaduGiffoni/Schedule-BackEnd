using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Schedule.Models.KnowledgeBase.Configurations
{
    /// <summary>
    /// Configuração Fluente (Fluent API) para a entidade KnowledgeMedia no SQL Server.
    /// Garante os limites de tamanho de URLs e a correta amarração com as versões do artigo (RB017).
    /// </summary>
    public class KnowledgeMediaConfiguration : IEntityTypeConfiguration<KnowledgeMedia>
    {
        public void Configure(EntityTypeBuilder<KnowledgeMedia> builder)
        {
            // Nome da Tabela na Base de Dados
            builder.ToTable("KnowledgeMedia");

            // Chave Primária
            builder.HasKey(x => x.Id);

            // Propriedades e Restrições
            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(255); // Tamanho padrão seguro para nomes de ficheiros

            builder.Property(x => x.FileUrl)
                .IsRequired()
                .HasMaxLength(2048); // URLs de SharePoint/Stream podem ser muito longas (RB018, RB019)

            builder.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>(); // Grava o Enum MediaType como um número inteiro na base de dados

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            // --- Índices de Performance ---

            // Índice para acelerar o carregamento das mídias ao abrir uma versão específica do artigo
            builder.HasIndex(x => x.ArticleVersionId);

            // --- Relacionamentos (Chaves Estrangeiras) ---

            // Relacionamento com a Versão do Artigo (N : 1)
            builder.HasOne(x => x.ArticleVersion)
                .WithMany() // Poderá alterar para .WithMany(v => v.Media) se descomentou a coleção na versão
                .HasForeignKey(x => x.ArticleVersionId)
                .OnDelete(DeleteBehavior.Cascade); // Se uma versão de artigo for eliminada, as referências das suas mídias também são apagadas.
        }
    }
}