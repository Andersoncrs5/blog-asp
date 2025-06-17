using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritePostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "favorite_comment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    ApplicationUserId1 = table.Column<string>(type: "text", nullable: true),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    CommentId1 = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorite_comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_favorite_comment_app_users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorite_comment_app_users_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "app_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_favorite_comment_comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_favorite_comment_comments_CommentId1",
                        column: x => x.CommentId1,
                        principalTable: "comments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_favorite_comment_ApplicationUserId",
                table: "favorite_comment",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_comment_ApplicationUserId1",
                table: "favorite_comment",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_comment_CommentId",
                table: "favorite_comment",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_favorite_comment_CommentId1",
                table: "favorite_comment",
                column: "CommentId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorite_comment");
        }
    }
}
