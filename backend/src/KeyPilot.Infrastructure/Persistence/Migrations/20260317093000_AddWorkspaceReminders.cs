using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyPilot.Infrastructure.Persistence.Migrations
{
    public partial class AddWorkspaceReminders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workspace_reminders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    property_id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    scheduled_for_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    sent_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_reminders", x => x.id);
                    table.ForeignKey(
                        name: "FK_workspace_reminders_properties_property_id",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workspace_reminders_key",
                table: "workspace_reminders",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_reminders_property_id",
                table: "workspace_reminders",
                column: "property_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workspace_reminders");
        }
    }
}