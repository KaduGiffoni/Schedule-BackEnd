using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schedule.Models;
using Schedule.Models.KnowledgeBase;
using Schedule.Models.Communication;
using Schedule.Models.Scheduling;

namespace Schedule.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // ==========================================
    // --- CORE & SCHEDULING ---
    // ==========================================
    public DbSet<Company> Companies { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Letter> Letters { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<Holiday> Holidays { get; set; }
    public DbSet<UserAbsence> UserAbsences { get; set; }
    public DbSet<SwapRequest> SwapRequests { get; set; }
    public DbSet<ScheduleDay> ScheduleDays { get; set; }
    public DbSet<ShiftPattern> ShiftPatterns { get; set; }

    // ==========================================
    // --- COMMUNICATION ---
    // ==========================================
    public DbSet<Notice> Notices { get; set; }
    public DbSet<NoticeAcknowledgment> NoticeByIdAcknowledgments { get; set; }
    public DbSet<NoticeComment> NoticeComments { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    // ==========================================
    // --- MÓDULO KNOWLEDGE BASE ---
    // ==========================================
    public DbSet<KnowledgeCategory> KnowledgeCategories { get; set; }
    public DbSet<KnowledgeArticle> KnowledgeArticles { get; set; }
    public DbSet<KnowledgeArticleVersion> KnowledgeArticleVersions { get; set; }
    public DbSet<KnowledgeTag> KnowledgeTags { get; set; }
    public DbSet<KnowledgeArticleTag> KnowledgeArticleTags { get; set; }
    public DbSet<KnowledgeArticleReference> KnowledgeArticleReferences { get; set; }
    public DbSet<KnowledgeView> KnowledgeViews { get; set; }
    public DbSet<KnowledgeFavorite> KnowledgeFavorites { get; set; }
    public DbSet<KnowledgeArticleRead> KnowledgeArticleReads { get; set; }
    public DbSet<KnowledgeHistory> KnowledgeHistories { get; set; }
    public DbSet<KnowledgeComment> KnowledgeComments { get; set; }
    public DbSet<KnowledgeBadge> KnowledgeBadges { get; set; }
    public DbSet<UserKnowledgeBadge> UserKnowledgeBadges { get; set; }
    public DbSet<KnowledgeMedia> KnowledgeMedias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Fundamental para o funcionamento do Identity!

        // ==========================================
        // --- CONFIGURAÇÕES DE COMUNICAÇÃO ---
        // ==========================================
        modelBuilder.Entity<Notice>()
            .HasOne(n => n.CreatedByUser)
            .WithMany()
            .HasForeignKey(n => n.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.TargetUser)
            .WithMany()
            .HasForeignKey(n => n.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ==========================================
        // --- CONFIGURAÇÕES DE AGENDAMENTO (SHIFTS) ---
        // ==========================================

        // UserAbsence possui múltiplos caminhos até ApplicationUser
        modelBuilder.Entity<UserAbsence>()
            .HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserAbsence>()
            .HasOne(a => a.SubstituteUser)
            .WithMany()
            .HasForeignKey(a => a.SubstituteUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserAbsence>()
            .HasIndex(a => new { a.UserId, a.StartDate, a.EndDate });

        // SOLUÇÃO DO ERRO 1785: SwapRequest possui dois relacionamentos diretos com ApplicationUser.
        // Forçar Restrict impede que o SQL Server tente aplicar deleção em cascata circular.
        modelBuilder.Entity<SwapRequest>()
            .HasOne(s => s.RequestingUser)
            .WithMany()
            .HasForeignKey(s => s.RequestingUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SwapRequest>()
            .HasOne(s => s.TargetUser)
            .WithMany()
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ==========================================
        // --- INJEÇÃO DE CONFIGURAÇÕES (Automática) ---
        // ==========================================
        // Lê de forma limpa e automática todas as classes que herdam de IEntityTypeConfiguration<T>
        // localizadas neste Assembly (como as do módulo KnowledgeBase).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}