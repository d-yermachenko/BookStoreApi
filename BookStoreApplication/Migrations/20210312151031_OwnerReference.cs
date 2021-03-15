using Microsoft.EntityFrameworkCore.Migrations;

namespace BookStoreApi.Migrations
{
    public partial class OwnerReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Authors",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Books",
                nullable: true,
                maxLength: 450);

            migrationBuilder.Sql("UPDATE [Books] SET [OwnerId] = (Select id from AspNetUsers where username='admin') WHERE [OwnerId] IS NULL");
            migrationBuilder.Sql("UPDATE [Authors] SET [OwnerId] = (Select id from AspNetUsers where username='admin') WHERE [OwnerId] IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Authors",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Books",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Authors");

        }
    }
}
