using Microsoft.EntityFrameworkCore.Migrations;

namespace IdeaApp.Migrations
{
    public partial class addingfieldgravatarimage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GravatarImageUrl",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GravatarImageUrl",
                table: "AspNetUsers");
        }
    }
}
