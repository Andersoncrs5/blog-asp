using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comments_app_users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comments_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoritePostEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    ApplicationUserId1 = table.Column<string>(type: "text", nullable: true),
                    PostId = table.Column<long>(type: "bigint", nullable: false),
                    PostId1 = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritePostEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoritePostEntities_app_users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "app_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoritePostEntities_app_users_ApplicationUserId1",
                        column: x => x.ApplicationUserId1,
                        principalTable: "app_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoritePostEntities_posts_PostId",
                        column: x => x.PostId,
                        principalTable: "posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoritePostEntities_posts_PostId1",
                        column: x => x.PostId1,
                        principalTable: "posts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_comments_ApplicationUserId",
                table: "comments",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_ParentId",
                table: "comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_comments_PostId",
                table: "comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePostEntities_ApplicationUserId",
                table: "FavoritePostEntities",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePostEntities_ApplicationUserId1",
                table: "FavoritePostEntities",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePostEntities_PostId",
                table: "FavoritePostEntities",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoritePostEntities_PostId1",
                table: "FavoritePostEntities",
                column: "PostId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "FavoritePostEntities");
        }
    }
}
