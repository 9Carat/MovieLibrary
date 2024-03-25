using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class MovieUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FK_UserId",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FK_UserId",
                table: "Movies");
        }
    }
}
