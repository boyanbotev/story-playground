using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTurns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Turns",
                table: "StoryNodes",
                newName: "TransitionTurns");

            migrationBuilder.AddColumn<int>(
                name: "ContentTurns",
                table: "StoryNodes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentTurns",
                table: "StoryNodes");

            migrationBuilder.RenameColumn(
                name: "TransitionTurns",
                table: "StoryNodes",
                newName: "Turns");
        }
    }
}
