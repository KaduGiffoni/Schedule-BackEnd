using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoSistemaDeCargos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SwapRequests_TargetUserId",
                table: "SwapRequests");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDays_LetterId",
                table: "ScheduleDays");

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_TargetUserId_Status",
                table: "SwapRequests",
                columns: new[] { "TargetUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDays_LetterId_Date",
                table: "ScheduleDays",
                columns: new[] { "LetterId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SwapRequests_TargetUserId_Status",
                table: "SwapRequests");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDays_LetterId_Date",
                table: "ScheduleDays");

            migrationBuilder.CreateIndex(
                name: "IX_SwapRequests_TargetUserId",
                table: "SwapRequests",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDays_LetterId",
                table: "ScheduleDays",
                column: "LetterId");
        }
    }
}
