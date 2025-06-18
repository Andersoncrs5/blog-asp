using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddReactionCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reaction_posts_ApplicationUserId",
                table: "reaction_posts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "posts",
                type: "varchar(350)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "posts",
                type: "varchar(3500)",
                maxLength: 3000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 3000);

            migrationBuilder.CreateTable(
                name: "reaction_comment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    CommentId = table.Column<long>(type: "bigint", nullable: false),
                    Reaction = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reaction_comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reaction_comment_app_users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reaction_comment_comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reaction_posts_ApplicationUserId_PostId",
                table: "reaction_posts",
                columns: new[] { "ApplicationUserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reaction_comment_ApplicationUserId_CommentId",
                table: "reaction_comment",
                columns: new[] { "ApplicationUserId", "CommentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reaction_comment_CommentId",
                table: "reaction_comment",
                column: "CommentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reaction_comment");

            migrationBuilder.DropIndex(
                name: "IX_reaction_posts_ApplicationUserId_PostId",
                table: "reaction_posts");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "posts",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(350)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "posts",
                type: "text",
                maxLength: 3000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(3500)",
                oldMaxLength: 3000);

            migrationBuilder.CreateIndex(
                name: "IX_reaction_posts_ApplicationUserId",
                table: "reaction_posts",
                column: "ApplicationUserId");
        }
    }
}
