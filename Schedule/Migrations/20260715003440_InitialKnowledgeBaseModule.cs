using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule.Migrations
{
    /// <inheritdoc />
    public partial class InitialKnowledgeBaseModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowledgeCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeCategories_KnowledgeCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "KnowledgeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeBadges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeBadges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeBadges_KnowledgeCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "KnowledgeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserKnowledgeBadges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    BadgeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKnowledgeBadges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserKnowledgeBadges_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserKnowledgeBadges_KnowledgeBadges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "KnowledgeBadges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeArticleReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticleReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticleReads_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeArticleReferences",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferencedArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KnowledgeArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticleReferences", x => new { x.ArticleId, x.ReferencedArticleId });
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FavoriteCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrentVersionId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticles_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticles_KnowledgeCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "KnowledgeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeArticleTags",
                columns: table => new
                {
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KnowledgeArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticleTags", x => new { x.ArticleId, x.TagId });
                    table.ForeignKey(
                        name: "FK_KnowledgeArticleTags_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticleTags_KnowledgeArticles_KnowledgeArticleId",
                        column: x => x.KnowledgeArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KnowledgeArticleTags_KnowledgeTags_TagId",
                        column: x => x.TagId,
                        principalTable: "KnowledgeTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeArticleVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    EstimatedTimeInMinutes = table.Column<int>(type: "int", nullable: false),
                    ChangeDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EditorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeArticleVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticleVersions_AspNetUsers_EditorId",
                        column: x => x.EditorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnowledgeArticleVersions_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnowledgeComments_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnowledgeFavorites_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KnowledgeHistories_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeViews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KnowledgeViews_KnowledgeArticles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "KnowledgeArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArticleVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeMedia_KnowledgeArticleVersions_ArticleVersionId",
                        column: x => x.ArticleVersionId,
                        principalTable: "KnowledgeArticleVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleReads_ArticleId_UserId",
                table: "KnowledgeArticleReads",
                columns: new[] { "ArticleId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleReads_UserId",
                table: "KnowledgeArticleReads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleReferences_KnowledgeArticleId",
                table: "KnowledgeArticleReferences",
                column: "KnowledgeArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleReferences_ReferencedArticleId",
                table: "KnowledgeArticleReferences",
                column: "ReferencedArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_AuthorId",
                table: "KnowledgeArticles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_CategoryId",
                table: "KnowledgeArticles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_CurrentVersionId",
                table: "KnowledgeArticles",
                column: "CurrentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_CurrentVersionId1",
                table: "KnowledgeArticles",
                column: "CurrentVersionId1");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_IsDeleted",
                table: "KnowledgeArticles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_Slug",
                table: "KnowledgeArticles",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticles_Status",
                table: "KnowledgeArticles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleTags_KnowledgeArticleId",
                table: "KnowledgeArticleTags",
                column: "KnowledgeArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleTags_TagId",
                table: "KnowledgeArticleTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleVersions_ArticleId",
                table: "KnowledgeArticleVersions",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeArticleVersions_EditorId",
                table: "KnowledgeArticleVersions",
                column: "EditorId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeBadges_CategoryId",
                table: "KnowledgeBadges",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeCategories_ParentCategoryId",
                table: "KnowledgeCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeCategories_Slug",
                table: "KnowledgeCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeComments_ArticleId_CreatedAt",
                table: "KnowledgeComments",
                columns: new[] { "ArticleId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeComments_UserId",
                table: "KnowledgeComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeFavorites_ArticleId_UserId",
                table: "KnowledgeFavorites",
                columns: new[] { "ArticleId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeFavorites_UserId",
                table: "KnowledgeFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeHistories_ArticleId",
                table: "KnowledgeHistories",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeHistories_UserId_CreatedAt",
                table: "KnowledgeHistories",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeMedia_ArticleVersionId",
                table: "KnowledgeMedia",
                column: "ArticleVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeTags_Slug",
                table: "KnowledgeTags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeViews_ArticleId",
                table: "KnowledgeViews",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeViews_UserId_ViewedAt",
                table: "KnowledgeViews",
                columns: new[] { "UserId", "ViewedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserKnowledgeBadges_BadgeId",
                table: "UserKnowledgeBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserKnowledgeBadges_UserId_BadgeId",
                table: "UserKnowledgeBadges",
                columns: new[] { "UserId", "BadgeId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleReads_KnowledgeArticles_ArticleId",
                table: "KnowledgeArticleReads",
                column: "ArticleId",
                principalTable: "KnowledgeArticles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleReferences_KnowledgeArticles_ArticleId",
                table: "KnowledgeArticleReferences",
                column: "ArticleId",
                principalTable: "KnowledgeArticles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleReferences_KnowledgeArticles_KnowledgeArticleId",
                table: "KnowledgeArticleReferences",
                column: "KnowledgeArticleId",
                principalTable: "KnowledgeArticles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleReferences_KnowledgeArticles_ReferencedArticleId",
                table: "KnowledgeArticleReferences",
                column: "ReferencedArticleId",
                principalTable: "KnowledgeArticles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticles_KnowledgeArticleVersions_CurrentVersionId",
                table: "KnowledgeArticles",
                column: "CurrentVersionId",
                principalTable: "KnowledgeArticleVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticles_KnowledgeArticleVersions_CurrentVersionId1",
                table: "KnowledgeArticles",
                column: "CurrentVersionId1",
                principalTable: "KnowledgeArticleVersions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticleVersions_KnowledgeArticles_ArticleId",
                table: "KnowledgeArticleVersions");

            migrationBuilder.DropTable(
                name: "KnowledgeArticleReads");

            migrationBuilder.DropTable(
                name: "KnowledgeArticleReferences");

            migrationBuilder.DropTable(
                name: "KnowledgeArticleTags");

            migrationBuilder.DropTable(
                name: "KnowledgeComments");

            migrationBuilder.DropTable(
                name: "KnowledgeFavorites");

            migrationBuilder.DropTable(
                name: "KnowledgeHistories");

            migrationBuilder.DropTable(
                name: "KnowledgeMedia");

            migrationBuilder.DropTable(
                name: "KnowledgeViews");

            migrationBuilder.DropTable(
                name: "UserKnowledgeBadges");

            migrationBuilder.DropTable(
                name: "KnowledgeTags");

            migrationBuilder.DropTable(
                name: "KnowledgeBadges");

            migrationBuilder.DropTable(
                name: "KnowledgeArticles");

            migrationBuilder.DropTable(
                name: "KnowledgeArticleVersions");

            migrationBuilder.DropTable(
                name: "KnowledgeCategories");
        }
    }
}
