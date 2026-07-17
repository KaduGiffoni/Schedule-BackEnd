using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule.Migrations
{
    /// <inheritdoc />
    public partial class RefatoracaoKnowledgeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticleReads_AspNetUsers_UserId",
                table: "KnowledgeArticleReads");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticles_KnowledgeArticleVersions_CurrentVersionId1",
                table: "KnowledgeArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticleTags_KnowledgeArticles_KnowledgeArticleId",
                table: "KnowledgeArticleTags");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeViews_AspNetUsers_UserId",
                table: "KnowledgeViews");

            migrationBuilder.DropForeignKey(
                name: "FK_NoticeByIdAcknowledgments_AspNetUsers_UserId",
                table: "NoticeByIdAcknowledgments");

            migrationBuilder.DropForeignKey(
                name: "FK_NoticeComments_AspNetUsers_UserId",
                table: "NoticeComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Notices_Sectors_SectorId",
                table: "Notices");

            migrationBuilder.DropForeignKey(
                name: "FK_UserKnowledgeBadges_AspNetUsers_UserId",
                table: "UserKnowledgeBadges");

            migrationBuilder.DropIndex(
                name: "IX_SwapRequests_TargetUserId_Status",
                table: "SwapRequests");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDays_LetterId_Date",
                table: "ScheduleDays");

            migrationBuilder.RenameColumn(
                name: "KnowledgeArticleId",
                table: "KnowledgeArticleTags",
                newName: "KnowledgeTagId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeArticleTags_KnowledgeArticleId",
                table: "KnowledgeArticleTags",
                newName: "IX_KnowledgeArticleTags_KnowledgeTagId");

            migrationBuilder.RenameColumn(
                name: "CurrentVersionId1",
                table: "KnowledgeArticles",
                newName: "KnowledgeCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeArticles_CurrentVersionId1",
                table: "KnowledgeArticles",
                newName: "IX_KnowledgeArticles_KnowledgeCategoryId");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "KnowledgeTags",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "KnowledgeTags",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "KnowledgeTags",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "KnowledgeArticleVersionId",
                table: "KnowledgeMedia",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "KnowledgeArticles",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "KnowledgeArticles",
                type: "datetimeoffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_TargetUserId",
                table: "SwapRequests",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDays_LetterId",
                table: "ScheduleDays",
                column: "LetterId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeMedia_KnowledgeArticleVersionId",
                table: "KnowledgeMedia",
                column: "KnowledgeArticleVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleReads_AspNetUsers_UserId",
                table: "KnowledgeArticleReads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticles_KnowledgeCategories_KnowledgeCategoryId",
                table: "KnowledgeArticles",
                column: "KnowledgeCategoryId",
                principalTable: "KnowledgeCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleTags_KnowledgeTags_KnowledgeTagId",
                table: "KnowledgeArticleTags",
                column: "KnowledgeTagId",
                principalTable: "KnowledgeTags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeMedia_KnowledgeArticleVersions_KnowledgeArticleVersionId",
                table: "KnowledgeMedia",
                column: "KnowledgeArticleVersionId",
                principalTable: "KnowledgeArticleVersions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeViews_AspNetUsers_UserId",
                table: "KnowledgeViews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NoticeByIdAcknowledgments_AspNetUsers_UserId",
                table: "NoticeByIdAcknowledgments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoticeComments_AspNetUsers_UserId",
                table: "NoticeComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_Sectors_SectorId",
                table: "Notices",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserKnowledgeBadges_AspNetUsers_UserId",
                table: "UserKnowledgeBadges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticleReads_AspNetUsers_UserId",
                table: "KnowledgeArticleReads");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticles_KnowledgeCategories_KnowledgeCategoryId",
                table: "KnowledgeArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeArticleTags_KnowledgeTags_KnowledgeTagId",
                table: "KnowledgeArticleTags");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeMedia_KnowledgeArticleVersions_KnowledgeArticleVersionId",
                table: "KnowledgeMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_KnowledgeViews_AspNetUsers_UserId",
                table: "KnowledgeViews");

            migrationBuilder.DropForeignKey(
                name: "FK_NoticeByIdAcknowledgments_AspNetUsers_UserId",
                table: "NoticeByIdAcknowledgments");

            migrationBuilder.DropForeignKey(
                name: "FK_NoticeComments_AspNetUsers_UserId",
                table: "NoticeComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Notices_Sectors_SectorId",
                table: "Notices");

            migrationBuilder.DropForeignKey(
                name: "FK_UserKnowledgeBadges_AspNetUsers_UserId",
                table: "UserKnowledgeBadges");

            migrationBuilder.DropIndex(
                name: "IX_SwapRequests_TargetUserId",
                table: "SwapRequests");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDays_LetterId",
                table: "ScheduleDays");

            migrationBuilder.DropIndex(
                name: "IX_KnowledgeMedia_KnowledgeArticleVersionId",
                table: "KnowledgeMedia");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "KnowledgeTags");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "KnowledgeTags");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "KnowledgeTags");

            migrationBuilder.DropColumn(
                name: "KnowledgeArticleVersionId",
                table: "KnowledgeMedia");

            migrationBuilder.RenameColumn(
                name: "KnowledgeTagId",
                table: "KnowledgeArticleTags",
                newName: "KnowledgeArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeArticleTags_KnowledgeTagId",
                table: "KnowledgeArticleTags",
                newName: "IX_KnowledgeArticleTags_KnowledgeArticleId");

            migrationBuilder.RenameColumn(
                name: "KnowledgeCategoryId",
                table: "KnowledgeArticles",
                newName: "CurrentVersionId1");

            migrationBuilder.RenameIndex(
                name: "IX_KnowledgeArticles_KnowledgeCategoryId",
                table: "KnowledgeArticles",
                newName: "IX_KnowledgeArticles_CurrentVersionId1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "KnowledgeArticles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "KnowledgeArticles",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_TargetUserId_Status",
                table: "SwapRequests",
                columns: new[] { "TargetUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDays_LetterId_Date",
                table: "ScheduleDays",
                columns: new[] { "LetterId", "Date" });

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleReads_AspNetUsers_UserId",
                table: "KnowledgeArticleReads",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticles_KnowledgeArticleVersions_CurrentVersionId1",
                table: "KnowledgeArticles",
                column: "CurrentVersionId1",
                principalTable: "KnowledgeArticleVersions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeArticleTags_KnowledgeArticles_KnowledgeArticleId",
                table: "KnowledgeArticleTags",
                column: "KnowledgeArticleId",
                principalTable: "KnowledgeArticles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_KnowledgeViews_AspNetUsers_UserId",
                table: "KnowledgeViews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NoticeByIdAcknowledgments_AspNetUsers_UserId",
                table: "NoticeByIdAcknowledgments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NoticeComments_AspNetUsers_UserId",
                table: "NoticeComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_Sectors_SectorId",
                table: "Notices",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserKnowledgeBadges_AspNetUsers_UserId",
                table: "UserKnowledgeBadges",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
