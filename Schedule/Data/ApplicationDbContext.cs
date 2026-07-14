using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Schedule.Models;
using Schedule.Models.KnowledgeBase;

namespace Schedule.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Mapeando as nossas classes para virarem tabelas de verdade
        public DbSet<Company> Companies { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Letter> Letters { get; set; }

        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Holiday> Holidays { get; set; }

        public DbSet<UserAbsence> UserAbsences { get; set; }

        public DbSet<SwapRequest> SwapRequests { get; set; }
        public DbSet<ScheduleDay> ScheduleDays { get; set; }

        public DbSet<ShiftPattern> ShiftPatterns { get; set; }

        public DbSet<Notice> Notices { get; set; }
        public DbSet<NoticeAcknowledgment> NoticeByIdAcknowledgments { get; set; }
        public DbSet<NoticeComment> NoticeComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // ==========================================
        // --- MÓDULO KNOWLEDGE BASE (Fases 1 a 3) ---
        // ==========================================
        public DbSet<KnowledgeCategory> KnowledgeCategories { get; set; }
        public DbSet<KnowledgeTag> KnowledgeTags { get; set; }
        public DbSet<KnowledgeArticle> KnowledgeArticles { get; set; }
        public DbSet<KnowledgeArticleVersion> KnowledgeArticleVersions { get; set; }
        public DbSet<KnowledgeArticleTag> KnowledgeArticleTags { get; set; }
        public DbSet<KnowledgeArticleReference> KnowledgeArticleReferences { get; set; }
        public DbSet<KnowledgeMedia> KnowledgeMedia { get; set; }
        public DbSet<KnowledgeFavorite> KnowledgeFavorites { get; set; }
        public DbSet<KnowledgeView> KnowledgeViews { get; set; }
        public DbSet<KnowledgeComment> KnowledgeComments { get; set; }
        public DbSet<KnowledgeHistory> KnowledgeHistories { get; set; }
        public DbSet<KnowledgeArticleRead> KnowledgeArticleReads { get; set; }
        public DbSet<KnowledgeBadge> KnowledgeBadges { get; set; }
        public DbSet<UserKnowledgeBadge> UserKnowledgeBadges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


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


            modelBuilder.Entity<ScheduleDay>()
                .HasIndex(sd => new { sd.LetterId, sd.Date });


            modelBuilder.Entity<SwapRequest>()
                .HasIndex(sr => new { sr.TargetUserId, sr.Status });

            modelBuilder.Entity<SwapRequest>()
                .HasIndex(sr => sr.RequestingUserId);

            modelBuilder.Entity<Notice>()
                .HasOne(n => n.CreatedByUser)
                .WithMany()
                .HasForeignKey(n => n.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notice>()
                .HasOne(n => n.Sector)
                .WithMany()
                .HasForeignKey(n => n.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoticeAcknowledgment>()
                .HasOne(na => na.User)
                .WithMany()
                .HasForeignKey(na => na.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoticeComment>()
                .HasOne(nc => nc.User)
                .WithMany()
                .HasForeignKey(nc => nc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.TargetUser)
                .WithMany()
                .HasForeignKey(n => n.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserAbsence tem duas FKs para ApplicationUser (User e SubstituteUser).
            // Restrict nas duas evita erro de múltiplos caminhos de cascade no SQL Server.
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

            // ==========================================
            // --- INJEÇÃO DE CONFIGURAÇÕES (Fase 4) ---
            // ==========================================
            // Este comando encontra automaticamente as nossas classes de configuração 
            // (KnowledgeArticleConfiguration, etc) dentro deste Assembly e aplica-as.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }
}