using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyPilot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Phase1DomainAlignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE conditions SET status = 'satisfied' WHERE status = 'completed';");

            migrationBuilder.AddColumn<DateOnly>(
                name: "cancelled_date",
                table: "properties",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "settled_date",
                table: "properties",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "unconditional_date",
                table: "properties",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE conditions SET status = 'completed' WHERE status = 'satisfied';");

            migrationBuilder.DropColumn(
                name: "cancelled_date",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "settled_date",
                table: "properties");

            migrationBuilder.DropColumn(
                name: "unconditional_date",
                table: "properties");
        }
    }
}
