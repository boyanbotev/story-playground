using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class AdjustStoryNodeTableRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Stories_storyId",
                table: "Nodes");

            migrationBuilder.RenameColumn(
                name: "storyId",
                table: "Nodes",
                newName: "StoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_storyId",
                table: "Nodes",
                newName: "IX_Nodes_StoryId");

            migrationBuilder.AddColumn<int>(
                name: "ContentTurns",
                table: "Nodes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Nodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NodeType",
                table: "Nodes",
                type: "TEXT",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TransitionTurns",
                table: "Nodes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserGoal",
                table: "Nodes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Stories_StoryId",
                table: "Nodes",
                column: "StoryId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Stories_StoryId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "ContentTurns",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "NodeType",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "TransitionTurns",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "UserGoal",
                table: "Nodes");

            migrationBuilder.RenameColumn(
                name: "StoryId",
                table: "Nodes",
                newName: "storyId");

            migrationBuilder.RenameIndex(
                name: "IX_Nodes_StoryId",
                table: "Nodes",
                newName: "IX_Nodes_storyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Stories_storyId",
                table: "Nodes",
                column: "storyId",
                principalTable: "Stories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
