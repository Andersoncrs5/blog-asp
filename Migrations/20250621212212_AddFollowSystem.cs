using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddFollowSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "follows",
                columns: table => new
                {
                    FollowerId = table.Column<string>(type: "character varying(256)", nullable: false),
                    FollowedId = table.Column<string>(type: "character varying(256)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_follows", x => new { x.FollowerId, x.FollowedId });
                    table.ForeignKey(
                        name: "FK_follows_app_users_FollowedId",
                        column: x => x.FollowedId,
                        principalTable: "app_users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_follows_app_users_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "app_users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_follows_FollowedId",
                table: "follows",
                column: "FollowedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "follows");
        }
    }
}
