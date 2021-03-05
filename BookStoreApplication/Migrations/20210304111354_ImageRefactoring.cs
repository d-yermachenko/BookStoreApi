using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace BookStoreApi.Migrations
{
    public partial class ImageRefactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Books",
                type: $"nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Books");
        }
    }
}
