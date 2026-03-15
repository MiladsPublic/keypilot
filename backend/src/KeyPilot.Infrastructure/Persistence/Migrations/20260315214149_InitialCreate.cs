using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyPilot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: true),
                    address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    offer_accepted_date = table.Column<DateOnly>(type: "date", nullable: true),
                    settlement_date = table.Column<DateOnly>(type: "date", nullable: true),
                    purchase_price = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_properties", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "properties");
        }
    }
}
