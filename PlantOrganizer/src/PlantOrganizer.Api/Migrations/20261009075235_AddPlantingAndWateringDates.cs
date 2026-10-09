using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantOrganizer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantingAndWateringDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "LastWateredAt",
                table: "Plants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PlantedAt",
                table: "Plants",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastWateredAt",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "PlantedAt",
                table: "Plants");
        }
    }
}
