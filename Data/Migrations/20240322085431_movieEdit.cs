using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class movieEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReleaseDate",
                table: "Movies",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Movies",
                newName: "Poster");

            migrationBuilder.AddColumn<string>(
                name: "Plot",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Plot",
                table: "Movies");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Movies",
                newName: "ReleaseDate");

            migrationBuilder.RenameColumn(
                name: "Poster",
                table: "Movies",
                newName: "Description");
        }
    }
}
