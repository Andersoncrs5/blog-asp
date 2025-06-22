using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddUserConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_configs",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "character varying(256)", nullable: false),
                    ThemeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PrimaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    SecondaryColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    AccentColor = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    FontType = table.Column<int>(type: "integer", nullable: true),
                    FontSize = table.Column<int>(type: "integer", nullable: true),
                    LineHeight = table.Column<decimal>(type: "numeric", nullable: true),
                    LetterSpacing = table.Column<decimal>(type: "numeric", nullable: true),
                    BorderColor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BorderSize = table.Column<int>(type: "integer", nullable: true),
                    BorderRadius = table.Column<int>(type: "integer", nullable: true),
                    LayoutPreference = table.Column<int>(type: "integer", nullable: true),
                    ShowProfilePictureInComments = table.Column<bool>(type: "boolean", nullable: true),
                    EnableAnimations = table.Column<bool>(type: "boolean", nullable: true),
                    NotificationsEnabled = table.Column<bool>(type: "boolean", nullable: true),
                    TimeZone = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_configs", x => x.ApplicationUserId);
                    table.ForeignKey(
                        name: "FK_user_configs_app_users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "app_users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_configs");
        }
    }
}
