using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeyPilot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HardenBuyingMethodDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE properties SET buying_method = 'private_sale' WHERE buying_method IS NULL OR buying_method = '';");

            migrationBuilder.AlterColumn<string>(
                name: "buying_method",
                table: "properties",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValueSql: "'private_sale'",
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "buying_method",
                table: "properties",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldDefaultValueSql: "'private_sale'");
        }
    }
}
