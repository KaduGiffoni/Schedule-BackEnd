using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoPadroesDeTurnoESetores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultShiftPatternId",
                table: "Sectors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShiftPatterns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sequence = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftPatterns", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sectors_DefaultShiftPatternId",
                table: "Sectors",
                column: "DefaultShiftPatternId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_ShiftPatterns_DefaultShiftPatternId",
                table: "Sectors",
                column: "DefaultShiftPatternId",
                principalTable: "ShiftPatterns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_ShiftPatterns_DefaultShiftPatternId",
                table: "Sectors");

            migrationBuilder.DropTable(
                name: "ShiftPatterns");

            migrationBuilder.DropIndex(
                name: "IX_Sectors_DefaultShiftPatternId",
                table: "Sectors");

            migrationBuilder.DropColumn(
                name: "DefaultShiftPatternId",
                table: "Sectors");
        }
    }
}
