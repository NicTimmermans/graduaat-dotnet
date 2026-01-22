using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class updateaancustomuservoorwwaanvragen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassWordResetCode",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "PassWordResetCodeExpiry",
                table: "AspNetUsers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PassWordResetCodeHash",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassWordResetCodeExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PassWordResetCodeHash",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "PassWordResetCode",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
