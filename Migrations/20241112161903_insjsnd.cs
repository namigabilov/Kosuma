using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kosuma.Migrations
{
    /// <inheritdoc />
    public partial class insjsnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLiveNow",
                table: "LiveStreams",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLiveNow",
                table: "LiveStreams");
        }
    }
}
