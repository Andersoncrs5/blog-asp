using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class FixWarnForeignKeyFavoriteComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_favorite_comment_app_users_ApplicationUserId1",
                table: "favorite_comment");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_comment_comments_CommentId1",
                table: "favorite_comment");

            migrationBuilder.DropIndex(
                name: "IX_favorite_comment_ApplicationUserId1",
                table: "favorite_comment");

            migrationBuilder.DropIndex(
                name: "IX_favorite_comment_CommentId1",
                table: "favorite_comment");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "favorite_comment");

            migrationBuilder.DropColumn(
                name: "CommentId1",
                table: "favorite_comment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "favorite_comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CommentId1",
                table: "favorite_comment",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_favorite_comment_ApplicationUserId1",
                table: "favorite_comment",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_comment_CommentId1",
                table: "favorite_comment",
                column: "CommentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_comment_app_users_ApplicationUserId1",
                table: "favorite_comment",
                column: "ApplicationUserId1",
                principalTable: "app_users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_comment_comments_CommentId1",
                table: "favorite_comment",
                column: "CommentId1",
                principalTable: "comments",
                principalColumn: "Id");
        }
    }
}
