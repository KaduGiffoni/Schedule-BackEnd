using AutoMapper;
using Schedule.DTOs.KnowledgeBase.Requests;
using Schedule.DTOs.KnowledgeBase.Responses;
using Schedule.Models.KnowledgeBase;
using System.Linq;

namespace Schedule.Mapping;

public class KnowledgeBaseProfile : Profile
{
    public KnowledgeBaseProfile()
    {
        // ==========================================
        // --- CATEGORIAS ---
        // ==========================================
        CreateMap<KnowledgeCategory, KnowledgeCategoryResponse>();
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
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.Title : string.Empty))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.Summary : string.Empty))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.Difficulty : default))
            .ForMember(dest => dest.EstimatedTimeInMinutes, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.EstimatedTimeInMinutes : 0))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src =>
                src.Author != null ? src.Author.UserName : string.Empty))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                src.CreatedAt.DateTime))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                src.UpdatedAt.DateTime));

        // ==========================================
        // --- ARTIGOS (Detalhe Completo) ---
        // ==========================================
        CreateMap<KnowledgeArticle, KnowledgeArticleDetailResponse>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.Title : string.Empty))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.Summary : string.Empty))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.Content : string.Empty))
            .ForMember(dest => dest.Difficulty, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.Difficulty : default))
            .ForMember(dest => dest.EstimatedTimeInMinutes, opt => opt.MapFrom(src =>
                src.CurrentVersion != null ? src.CurrentVersion.EstimatedTimeInMinutes : 0))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src =>
                src.Author != null ? src.Author.UserName : string.Empty))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src =>
                src.CreatedAt.DateTime))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src =>
                src.UpdatedAt.DateTime))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.ArticleTags
                   .Where(at => at.Tag != null)
                   .Select(at => new KnowledgeTagResponse
                   {
                       Id = at.Tag!.Id,
                       Name = at.Tag.Name,
                       Slug = at.Tag.Slug
                   })))
            .ForMember(dest => dest.References, opt => opt.MapFrom(src =>
                src.References
                   .Where(r => r.ReferencedArticle != null)
                   .Select(r => new KnowledgeArticleSummaryResponse
                   {
                       Id = r.ReferencedArticle!.Id,
                       Slug = r.ReferencedArticle.Slug,
                       Status = r.ReferencedArticle.Status,
                       ViewCount = r.ReferencedArticle.ViewCount,
                       FavoriteCount = r.ReferencedArticle.FavoriteCount,
                       CreatedAt = r.ReferencedArticle.CreatedAt.DateTime,
                       LastUpdatedAt = r.ReferencedArticle.UpdatedAt.DateTime,
                       CategoryId = r.ReferencedArticle.CategoryId,
                       Title = r.ReferencedArticle.CurrentVersion != null ? r.ReferencedArticle.CurrentVersion.Title : string.Empty,
                       Summary = r.ReferencedArticle.CurrentVersion != null ? r.ReferencedArticle.CurrentVersion.Summary : string.Empty,
                       Difficulty = r.ReferencedArticle.CurrentVersion != null ? r.ReferencedArticle.CurrentVersion.Difficulty : default,
                       EstimatedTimeInMinutes = r.ReferencedArticle.CurrentVersion != null ? r.ReferencedArticle.CurrentVersion.EstimatedTimeInMinutes : 0,
                       CategoryName = r.ReferencedArticle.Category != null ? r.ReferencedArticle.Category.Name : string.Empty,
                       AuthorName = r.ReferencedArticle.Author != null ? r.ReferencedArticle.Author.UserName : string.Empty
                   })));

        CreateMap<CreateKnowledgeArticleRequest, KnowledgeArticle>();
        CreateMap<UpdateKnowledgeArticleRequest, KnowledgeArticle>();
    }
}