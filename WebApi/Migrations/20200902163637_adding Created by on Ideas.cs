using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaApp.Migrations
{
    public partial class addingCreatedbyonIdeas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Ideas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_CreatedById",
                table: "Ideas",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_AspNetUsers_CreatedById",
                table: "Ideas",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_AspNetUsers_CreatedById",
                table: "Ideas");

            migrationBuilder.DropIndex(
                name: "IX_Ideas_CreatedById",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Ideas");
        }
    }
}
