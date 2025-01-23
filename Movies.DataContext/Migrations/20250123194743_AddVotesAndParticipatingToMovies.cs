using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddVotesAndParticipatingToMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsParticipating",
                table: "Movies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Votes",
                table: "Movies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1,
                columns: new[] { "IsParticipating", "Votes" },
                values: new object[] { false, 0 });

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2,
                columns: new[] { "IsParticipating", "Votes" },
                values: new object[] { false, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsParticipating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Votes",
                table: "Movies");
        }
    }
}
