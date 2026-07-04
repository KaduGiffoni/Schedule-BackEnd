using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule.Migrations
{
    /// <inheritdoc />
    public partial class AddSectorIsolationToNotices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectorId",
                table: "Notices",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserAbsences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubstituteUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAbsences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAbsences_AspNetUsers_SubstituteUserId",
                        column: x => x.SubstituteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAbsences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notices_SectorId",
                table: "Notices",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAbsences_SubstituteUserId",
                table: "UserAbsences",
                column: "SubstituteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAbsences_UserId_StartDate_EndDate",
                table: "UserAbsences",
                columns: new[] { "UserId", "StartDate", "EndDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_Sectors_SectorId",
                table: "Notices",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notices_Sectors_SectorId",
                table: "Notices");

            migrationBuilder.DropTable(
                name: "UserAbsences");

            migrationBuilder.DropIndex(
                name: "IX_Notices_SectorId",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "SectorId",
                table: "Notices");
        }
    }
}
