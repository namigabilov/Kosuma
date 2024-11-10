using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kosuma.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WritedAt",
                table: "LiveChats");

            migrationBuilder.AddColumn<int>(
                name: "WritedTime",
                table: "LiveChats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WritedTime",
                table: "LiveChats");

            migrationBuilder.AddColumn<DateTime>(
                name: "WritedAt",
                table: "LiveChats",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
