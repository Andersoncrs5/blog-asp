using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddReactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoritePostEntities_app_users_ApplicationUserId",
                table: "FavoritePostEntities");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoritePostEntities_posts_PostId",
                table: "FavoritePostEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoritePostEntities",
                table: "FavoritePostEntities");

            migrationBuilder.RenameTable(
                name: "FavoritePostEntities",
                newName: "favorite_post");

            migrationBuilder.RenameIndex(
                name: "IX_FavoritePostEntities_PostId",
                table: "favorite_post",
                newName: "IX_favorite_post_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoritePostEntities_ApplicationUserId",
                table: "favorite_post",
                newName: "IX_favorite_post_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_favorite_post",
                table: "favorite_post",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "reaction_posts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    Reaction = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reaction_posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reaction_posts_app_users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reaction_posts_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reaction_posts_ApplicationUserId",
                table: "reaction_posts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_reaction_posts_PostId",
                table: "reaction_posts",
                column: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_post_app_users_ApplicationUserId",
                table: "favorite_post",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_post_posts_PostId",
                table: "favorite_post",
                column: "PostId",
                principalTable: "posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_post_app_users_ApplicationUserId",
                table: "favorite_post");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_post_posts_PostId",
                table: "favorite_post");

            migrationBuilder.DropTable(
                name: "reaction_posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_favorite_post",
                table: "favorite_post");

            migrationBuilder.RenameTable(
                name: "favorite_post",
                newName: "FavoritePostEntities");

            migrationBuilder.RenameIndex(
                name: "IX_favorite_post_PostId",
                table: "FavoritePostEntities",
                newName: "IX_FavoritePostEntities_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_favorite_post_ApplicationUserId",
                table: "FavoritePostEntities",
                newName: "IX_FavoritePostEntities_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoritePostEntities",
                table: "FavoritePostEntities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoritePostEntities_app_users_ApplicationUserId",
                table: "FavoritePostEntities",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoritePostEntities_posts_PostId",
                table: "FavoritePostEntities",
                column: "PostId",
                principalTable: "posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
