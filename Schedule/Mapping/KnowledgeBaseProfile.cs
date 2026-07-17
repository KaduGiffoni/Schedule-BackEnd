using AutoMapper;
using Schedule.DTOs.KnowledgeBase.Requests;
using Schedule.DTOs.KnowledgeBase.Responses;
using Schedule.Models.KnowledgeBase;
using System.Linq;

namespace Schedule.Mapping; // File-scoped namespace, removido JSType acidental

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
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Title : string.Empty))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Summary : string.Empty))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Difficulty : default))
            .ForMember(dest => dest.EstimatedTimeInMinutes, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.EstimatedTimeInMinutes : 0))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.UserName : string.Empty));

        // ==========================================
        // --- ARTIGOS (Detalhe Completo) ---
        // ==========================================
        CreateMap<KnowledgeArticle, KnowledgeArticleDetailResponse>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Title : string.Empty))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Summary : string.Empty))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Content : string.Empty))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.Difficulty : default))
            .ForMember(dest => dest.EstimatedTimeInMinutes, opt => opt.MapFrom(src => src.CurrentVersion != null ? src.CurrentVersion.EstimatedTimeInMinutes : 0))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author != null ? src.Author.UserName : string.Empty))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ArticleTags.Select(at => at.Tag)))
            .ForMember(dest => dest.References, opt => opt.MapFrom(src => src.References.Select(r => r.ReferencedArticle)));

        CreateMap<CreateKnowledgeArticleRequest, KnowledgeArticle>();
        CreateMap<UpdateKnowledgeArticleRequest, KnowledgeArticle>();
    }
}