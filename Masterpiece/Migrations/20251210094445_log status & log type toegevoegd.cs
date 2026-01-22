using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class logstatuslogtypetoegevoegd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Log");

            migrationBuilder.AddColumn<string>(
                name: "LogStatus",
                table: "Log",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LogType",
                table: "Log",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogStatus",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "LogType",
                table: "Log");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Log",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
