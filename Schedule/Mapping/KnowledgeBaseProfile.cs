using AutoMapper;
using Schedule.DTOs.KnowledgeBase.Requests;
using Schedule.DTOs.KnowledgeBase.Responses;
using Schedule.Models.KnowledgeBase;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Schedule.Mapping
{
    /// <summary>
    /// Perfil de mapeamento do AutoMapper para o módulo Knowledge Base.
    /// Define as regras de transformação entre Entidades de Domínio e DTOs.
    /// </summary>
    public class KnowledgeBaseProfile : Profile
    {
        public KnowledgeBaseProfile()
        {
            // ==========================================
            // --- CATEGORIAS ---
            // ==========================================
            CreateMap<KnowledgeCategory, KnowledgeCategoryResponse>();

            // As requisições de criação e atualização transformam-se em entidades
            CreateMap<CreateKnowledgeCategoryRequest, KnowledgeCategory>();
            CreateMap<UpdateKnowledgeCategoryRequest, KnowledgeCategory>();

            // ==========================================
            // --- TAGS ---
            // ==========================================
            CreateMap<KnowledgeTag, KnowledgeTagResponse>();

            // ==========================================
            // --- ARTIGOS (Resumo e Listagem) ---
            // ==========================================
            CreateMap<KnowledgeArticle, KnowledgeArticleSummaryResponse>()
                // Extrai os dados que estão na versão atual (RB004)
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Title : string.Empty))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Summary : string.Empty))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Difficulty : default))
                .ForMember(dest => dest.EstimatedTimeInMinutes, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.EstimatedTimeInMinutes : 0))

                // Achata os nomes das entidades relacionadas
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                // Assumindo que ApplicationUser tem uma propriedade 'Name' ou 'FullName'
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.UserName : string.Empty));

            // ==========================================
            // --- ARTIGOS (Detalhe Completo) ---
            // ==========================================
            CreateMap<KnowledgeArticle, KnowledgeArticleDetailResponse>()
                // Extrai os dados e o conteúdo Markdown/HTML da versão atual
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Title : string.Empty))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Summary : string.Empty))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Content : string.Empty))
                .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Difficulty : default))
                .ForMember(dest => dest.EstimatedTimeInMinutes, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.EstimatedTimeInMinutes : 0))

                // Achata nomes das entidades raiz
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.UserName : string.Empty))

                // Mapeia a lista de Tags navegando pela tabela de junção
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ArticleTags.Select(at => at.Tag)))

                // Mapeia os pré-requisitos (RB031) reaproveitando o mapa de Resumo
                .ForMember(dest => dest.References, opt => opt.MapFrom(src => src.References.Select(r => r.ReferencedArticle)));

            // Requisão de Criação de Artigo para Entidade (Apenas dados base, a Versão é criada no Service)
            CreateMap<CreateKnowledgeArticleRequest, KnowledgeArticle>();
        }
    }
}