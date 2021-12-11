using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoWebAPI.Data.Migrations
{
    public partial class RenamedNameColumnToTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ToDos",
                newName: "Title");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ToDos",
                newName: "Name");
        }
    }
}
