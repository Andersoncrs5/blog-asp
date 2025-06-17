using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class FixWarnForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoritePostEntities_app_users_ApplicationUserId1",
                table: "FavoritePostEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoritePostEntities_posts_PostId1",
                table: "FavoritePostEntities");

            migrationBuilder.DropIndex(
                name: "IX_FavoritePostEntities_ApplicationUserId1",
                table: "FavoritePostEntities");

            migrationBuilder.DropIndex(
                name: "IX_FavoritePostEntities_PostId1",
                table: "FavoritePostEntities");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "FavoritePostEntities");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "FavoritePostEntities");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "posts",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "posts",
                type: "text",
                maxLength: 3000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3000)",
                oldMaxLength: 3000);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "posts",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "posts",
                type: "character varying(3000)",
                maxLength: 3000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 3000);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "FavoritePostEntities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PostId1",
                table: "FavoritePostEntities",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePostEntities_ApplicationUserId1",
                table: "FavoritePostEntities",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePostEntities_PostId1",
                table: "FavoritePostEntities",
                column: "PostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoritePostEntities_app_users_ApplicationUserId1",
                table: "FavoritePostEntities",
                column: "ApplicationUserId1",
                principalTable: "app_users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoritePostEntities_posts_PostId1",
                table: "FavoritePostEntities",
                column: "PostId1",
                principalTable: "posts",
                principalColumn: "Id");
        }
    }
}
