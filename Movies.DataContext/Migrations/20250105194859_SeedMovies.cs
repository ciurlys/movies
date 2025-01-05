using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Movies.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class SeedMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "MovieId", "Description", "Director", "ImagePath", "ReleaseDate", "Seen", "Title" },
                values: new object[,]
                {
                    { 1, "A thriller about dreams", "Christopher Nolan", "inception.jpg", new DateOnly(2010, 7, 16), true, "Inception" },
                    { 2, "You know what it is", "The Wachowskis", "matrix.jpg", new DateOnly(1999, 3, 31), true, "The Matrix" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "MovieId",
                keyValue: 2);
        }
    }
}
