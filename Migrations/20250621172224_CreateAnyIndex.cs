using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class CreateAnyIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_app_user_claims_app_users_UserId",
                table: "app_user_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_app_user_logins_app_users_UserId",
                table: "app_user_logins");

            migrationBuilder.DropForeignKey(
                name: "FK_app_user_roles_app_users_UserId",
                table: "app_user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_app_user_tokens_app_users_UserId",
                table: "app_user_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_app_users_ApplicationUserId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_comment_app_users_ApplicationUserId",
                table: "favorite_comment");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_post_app_users_ApplicationUserId",
                table: "favorite_post");

            migrationBuilder.DropForeignKey(
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_reaction_comment_app_users_ApplicationUserId",
                table: "reaction_comment");

            migrationBuilder.DropForeignKey(
                name: "FK_reaction_posts_app_users_ApplicationUserId",
                table: "reaction_posts");

            migrationBuilder.DropForeignKey(
                name: "FK_recover_account_app_users_ApplicationUserId",
                table: "recover_account");

            migrationBuilder.DropForeignKey(
                name: "FK_recover_account_app_users_UserId",
                table: "recover_account");

            migrationBuilder.DropForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric");

            migrationBuilder.DropIndex(
                name: "IX_recover_account_UserId",
                table: "recover_account");

            migrationBuilder.DropPrimaryKey(
                name: "PK_app_users",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "recover_account");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_metric",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "recover_account",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "recover_account",
                type: "character varying(256)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "reaction_posts",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "reaction_comment",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "posts",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "play_lists",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "favorite_post",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "favorite_comment",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "comments",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "categories",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "app_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "app_users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_tokens",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_roles",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_logins",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_claims",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_app_users",
                table: "app_users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_user_metric_ApplicationUserId",
                table: "user_metric",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_recover_account_UserEmail",
                table: "recover_account",
                column: "UserEmail");

            migrationBuilder.CreateIndex(
                name: "IX_media_post_Url",
                table: "media_post",
                column: "Url");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMetricEntities_ReportCount",
                table: "CommentMetricEntities",
                column: "ReportCount");

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_claims_app_users_UserId",
                table: "app_user_claims",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_logins_app_users_UserId",
                table: "app_user_logins",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_roles_app_users_UserId",
                table: "app_user_roles",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_tokens_app_users_UserId",
                table: "app_user_tokens",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_app_users_ApplicationUserId",
                table: "categories",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_comment_app_users_ApplicationUserId",
                table: "favorite_comment",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_post_app_users_ApplicationUserId",
                table: "favorite_post",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reaction_comment_app_users_ApplicationUserId",
                table: "reaction_comment",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reaction_posts_app_users_ApplicationUserId",
                table: "reaction_posts",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recover_account_app_users_ApplicationUserId",
                table: "recover_account",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recover_account_app_users_UserEmail",
                table: "recover_account",
                column: "UserEmail",
                principalTable: "app_users",
                principalColumn: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_app_user_claims_app_users_UserId",
                table: "app_user_claims");

            migrationBuilder.DropForeignKey(
                name: "FK_app_user_logins_app_users_UserId",
                table: "app_user_logins");

            migrationBuilder.DropForeignKey(
                name: "FK_app_user_roles_app_users_UserId",
                table: "app_user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_app_user_tokens_app_users_UserId",
                table: "app_user_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_app_users_ApplicationUserId",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_comment_app_users_ApplicationUserId",
                table: "favorite_comment");

            migrationBuilder.DropForeignKey(
                name: "FK_favorite_post_app_users_ApplicationUserId",
                table: "favorite_post");

            migrationBuilder.DropForeignKey(
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_reaction_comment_app_users_ApplicationUserId",
                table: "reaction_comment");

            migrationBuilder.DropForeignKey(
                name: "FK_reaction_posts_app_users_ApplicationUserId",
                table: "reaction_posts");

            migrationBuilder.DropForeignKey(
                name: "FK_recover_account_app_users_ApplicationUserId",
                table: "recover_account");

            migrationBuilder.DropForeignKey(
                name: "FK_recover_account_app_users_UserEmail",
                table: "recover_account");

            migrationBuilder.DropForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric");

            migrationBuilder.DropIndex(
                name: "IX_user_metric_ApplicationUserId",
                table: "user_metric");

            migrationBuilder.DropIndex(
                name: "IX_recover_account_UserEmail",
                table: "recover_account");

            migrationBuilder.DropIndex(
                name: "IX_media_post_Url",
                table: "media_post");

            migrationBuilder.DropIndex(
                name: "IX_CommentMetricEntities_ReportCount",
                table: "CommentMetricEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_app_users",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "recover_account");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_metric",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "recover_account",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "recover_account",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "reaction_posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "reaction_comment",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "play_lists",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "favorite_post",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "favorite_comment",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "comments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "categories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "app_users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "app_users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "app_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_tokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_roles",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_logins",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "app_user_claims",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_app_users",
                table: "app_users",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_recover_account_UserId",
                table: "recover_account",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_claims_app_users_UserId",
                table: "app_user_claims",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_logins_app_users_UserId",
                table: "app_user_logins",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_roles_app_users_UserId",
                table: "app_user_roles",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_app_user_tokens_app_users_UserId",
                table: "app_user_tokens",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_app_users_ApplicationUserId",
                table: "categories",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_app_users_ApplicationUserId",
                table: "comments",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_favorite_comment_app_users_ApplicationUserId",
                table: "favorite_comment",
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
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reaction_comment_app_users_ApplicationUserId",
                table: "reaction_comment",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reaction_posts_app_users_ApplicationUserId",
                table: "reaction_posts",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recover_account_app_users_ApplicationUserId",
                table: "recover_account",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_recover_account_app_users_UserId",
                table: "recover_account",
                column: "UserId",
                principalTable: "app_users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
