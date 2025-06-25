using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_posts_app_users_ApplicationUserId1",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_CategoryId1",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_recover_account_app_users_UserId",
                table: "recover_account");

            migrationBuilder.DropIndex(
                name: "IX_recover_account_UserId",
                table: "recover_account");

            migrationBuilder.DropIndex(
                name: "IX_posts_ApplicationUserId1",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_CategoryId1",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "recover_account");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "recover_account",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "posts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CategoryId1",
                table: "posts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_recover_account_UserId",
                table: "recover_account",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_posts_ApplicationUserId1",
                table: "posts",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_posts_CategoryId1",
                table: "posts",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_app_users_ApplicationUserId1",
                table: "posts",
                column: "ApplicationUserId1",
                principalTable: "app_users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_CategoryId1",
                table: "posts",
                column: "CategoryId1",
                principalTable: "categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_recover_account_app_users_UserId",
                table: "recover_account",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id");
        }
    }
}
