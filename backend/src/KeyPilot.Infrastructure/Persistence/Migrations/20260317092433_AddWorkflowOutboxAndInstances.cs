using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyPilot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkflowOutboxAndInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "workspace_workflow_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    deduplication_key = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    task_id = table.Column<Guid>(type: "uuid", nullable: true),
                    condition_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_error = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_workflow_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "workspace_workflow_instances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    workspace_id = table.Column<Guid>(type: "uuid", nullable: false),
                    workflow_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_signaled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    closed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_workspace_workflow_instances", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workspace_workflow_events_deduplication_key",
                table: "workspace_workflow_events",
                column: "deduplication_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_workspace_workflow_events_status_created_at_utc",
                table: "workspace_workflow_events",
                columns: new[] { "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_workspace_workflow_events_workspace_id",
                table: "workspace_workflow_events",
                column: "workspace_id");

            migrationBuilder.CreateIndex(
                name: "IX_workspace_workflow_instances_workspace_id",
                table: "workspace_workflow_instances",
                column: "workspace_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "workspace_workflow_events");

            migrationBuilder.DropTable(
                name: "workspace_workflow_instances");
        }
    }
}
