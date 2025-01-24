using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Movies.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddVoteTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateId",
                table: "Dates",
                newName: "VoteDateId");

            migrationBuilder.CreateTable(
                name: "UserVotesDate",
                columns: table => new
                {
                    UserVoteDateId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DateId = table.Column<int>(type: "integer", nullable: false),
                    HasVoted = table.Column<bool>(type: "boolean", nullable: false),
                    VoteDateId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVotesDate", x => x.UserVoteDateId);
                    table.ForeignKey(
                        name: "FK_UserVotesDate_Dates_VoteDateId",
                        column: x => x.VoteDateId,
                        principalTable: "Dates",
                        principalColumn: "VoteDateId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserVotesDate_UserId_DateId",
                table: "UserVotesDate",
                columns: new[] { "UserId", "DateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserVotesDate_VoteDateId",
                table: "UserVotesDate",
                column: "VoteDateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserVotesDate");

            migrationBuilder.RenameColumn(
                name: "VoteDateId",
                table: "Dates",
                newName: "DateId");
        }
    }
}
