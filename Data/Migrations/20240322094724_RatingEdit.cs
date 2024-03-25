using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class RatingEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Ratings",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "Reviewer",
                table: "Ratings",
                newName: "Source");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Ratings",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "Source",
                table: "Ratings",
                newName: "Reviewer");
        }
    }
}
