using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddUnreadComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCommentReads",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CommentId = table.Column<int>(type: "integer", nullable: false),
                    Seen = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommentReads", x => new { x.UserId, x.CommentId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCommentReads");
        }
    }
}
