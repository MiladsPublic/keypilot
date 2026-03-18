using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyPilot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExtendLifecycleEnrichTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "task_id",
                table: "workspace_reminders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "tasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "importance",
                table: "tasks",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValueSql: "'recommended'");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "tasks",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "settlement_date",
                table: "properties",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "accepted_offer_date",
                table: "properties",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            // Migrate old enum values to new names
            migrationBuilder.Sql("UPDATE properties SET status = 'conditional' WHERE status = 'accepted_offer'");
            migrationBuilder.Sql("UPDATE properties SET status = 'settlement_pending' WHERE status = 'pre_settlement'");
            migrationBuilder.Sql("UPDATE tasks SET stage = 'submitted' WHERE stage = 'acceptedoffer' OR stage = 'accepted_offer'");
            migrationBuilder.Sql("UPDATE tasks SET stage = 'settlement_pending' WHERE stage = 'presettlement' OR stage = 'pre_settlement'");
            migrationBuilder.Sql("UPDATE tasks SET status = 'needs_attention' WHERE status = 'needsattention'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_id",
                table: "workspace_reminders");

            migrationBuilder.DropColumn(
                name: "description",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "importance",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "tasks");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "settlement_date",
                table: "properties",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "accepted_offer_date",
                table: "properties",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
