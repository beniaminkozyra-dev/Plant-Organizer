using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantOrganizer.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LatinName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    WateringIntervalDays = table.Column<int>(type: "INTEGER", nullable: false),
                    SunRequirement = table.Column<int>(type: "INTEGER", nullable: false),
                    MinPotVolumeLiters = table.Column<decimal>(type: "TEXT", nullable: false),
                    DaysToHarvest = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plants");
        }
    }
}
