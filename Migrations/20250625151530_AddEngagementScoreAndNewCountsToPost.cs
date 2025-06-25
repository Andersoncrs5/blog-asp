using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class AddEngagementScoreAndNewCountsToPost : Migration
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
                name: "FK_follows_app_users_FollowedId",
                table: "follows");

            migrationBuilder.DropForeignKey(
                name: "FK_follows_app_users_FollowerId",
                table: "follows");

            migrationBuilder.DropForeignKey(
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists");

            migrationBuilder.DropForeignKey(
                name: "FK_playlist_items_play_lists_PlaylistId",
                table: "playlist_items");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_categoryId",
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
                name: "FK_user_configs_app_users_ApplicationUserId",
                table: "user_configs");

            migrationBuilder.DropForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric");

            migrationBuilder.DropForeignKey(
                name: "FK_user_preferences_app_users_ApplicationUserId",
                table: "user_preferences");

            migrationBuilder.DropIndex(
                name: "IX_recover_account_UserEmail",
                table: "recover_account");

            migrationBuilder.DropPrimaryKey(
                name: "PK_app_users",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "recover_account");

            migrationBuilder.RenameColumn(
                name: "categoryId",
                table: "posts",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_categoryId",
                table: "posts",
                newName: "IX_posts_CategoryId");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_preferences",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_metric",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AddColumn<long>(
                name: "preference_count",
                table: "user_metric",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "BorderColor",
                table: "user_configs",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_configs",
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
                name: "Title",
                table: "posts",
                type: "character varying(350)",
                maxLength: 350,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(350)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "posts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

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

            migrationBuilder.AddColumn<double>(
                name: "EngagementScore",
                table: "posts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "play_lists",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "play_lists",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "play_lists",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1500)",
                oldMaxLength: 1500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "play_lists",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AddColumn<byte>(
                name: "ItemCount",
                table: "play_lists",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "media_post",
                type: "character varying(1250)",
                maxLength: 1250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "media_post",
                type: "character varying(600)",
                maxLength: 600,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FollowedId",
                table: "follows",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AlterColumn<string>(
                name: "FollowerId",
                table: "follows",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)");

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveNotifications",
                table: "follows",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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
                name: "Content",
                table: "comments",
                type: "character varying(800)",
                maxLength: 800,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

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
                name: "IX_user_metric_preference_count",
                table: "user_metric",
                column: "preference_count");

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

            migrationBuilder.CreateIndex(
                name: "IX_media_post_MediaType",
                table: "media_post",
                column: "MediaType");

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
                name: "FK_follows_app_users_FollowedId",
                table: "follows",
                column: "FollowedId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_follows_app_users_FollowerId",
                table: "follows",
                column: "FollowerId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_playlist_items_play_lists_PlaylistId",
                table: "playlist_items",
                column: "PlaylistId",
                principalTable: "play_lists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_app_users_ApplicationUserId1",
                table: "posts",
                column: "ApplicationUserId1",
                principalTable: "app_users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_CategoryId",
                table: "posts",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_CategoryId1",
                table: "posts",
                column: "CategoryId1",
                principalTable: "categories",
                principalColumn: "Id");

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
                name: "FK_user_configs_app_users_ApplicationUserId",
                table: "user_configs",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_preferences_app_users_ApplicationUserId",
                table: "user_preferences",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Id",
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
                name: "FK_follows_app_users_FollowedId",
                table: "follows");

            migrationBuilder.DropForeignKey(
                name: "FK_follows_app_users_FollowerId",
                table: "follows");

            migrationBuilder.DropForeignKey(
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists");

            migrationBuilder.DropForeignKey(
                name: "FK_playlist_items_play_lists_PlaylistId",
                table: "playlist_items");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_app_users_ApplicationUserId1",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_CategoryId",
                table: "posts");

            migrationBuilder.DropForeignKey(
                name: "FK_posts_categories_CategoryId1",
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
                name: "FK_user_configs_app_users_ApplicationUserId",
                table: "user_configs");

            migrationBuilder.DropForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric");

            migrationBuilder.DropForeignKey(
                name: "FK_user_preferences_app_users_ApplicationUserId",
                table: "user_preferences");

            migrationBuilder.DropIndex(
                name: "IX_user_metric_preference_count",
                table: "user_metric");

            migrationBuilder.DropIndex(
                name: "IX_recover_account_UserId",
                table: "recover_account");

            migrationBuilder.DropIndex(
                name: "IX_posts_ApplicationUserId1",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_CategoryId1",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_media_post_MediaType",
                table: "media_post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_app_users",
                table: "app_users");

            migrationBuilder.DropColumn(
                name: "preference_count",
                table: "user_metric");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "recover_account");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "EngagementScore",
                table: "posts");

            migrationBuilder.DropColumn(
                name: "ItemCount",
                table: "play_lists");

            migrationBuilder.DropColumn(
                name: "ReceiveNotifications",
                table: "follows");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "posts",
                newName: "categoryId");

            migrationBuilder.RenameIndex(
                name: "IX_posts_CategoryId",
                table: "posts",
                newName: "IX_posts_categoryId");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_preferences",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_metric",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BorderColor",
                table: "user_configs",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(7)",
                oldMaxLength: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "user_configs",
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
                name: "Title",
                table: "posts",
                type: "varchar(350)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(350)",
                oldMaxLength: 350);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "posts",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "play_lists",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "play_lists",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "play_lists",
                type: "character varying(1500)",
                maxLength: 1500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "play_lists",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "media_post",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1250)",
                oldMaxLength: 1250);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "media_post",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(600)",
                oldMaxLength: 600,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FollowedId",
                table: "follows",
                type: "character varying(256)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FollowerId",
                table: "follows",
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
                name: "Content",
                table: "comments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(800)",
                oldMaxLength: 800);

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
                name: "IX_recover_account_UserEmail",
                table: "recover_account",
                column: "UserEmail");

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
                name: "FK_follows_app_users_FollowedId",
                table: "follows",
                column: "FollowedId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_follows_app_users_FollowerId",
                table: "follows",
                column: "FollowerId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_play_lists_app_users_ApplicationUserId",
                table: "play_lists",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_playlist_items_play_lists_PlaylistId",
                table: "playlist_items",
                column: "PlaylistId",
                principalTable: "play_lists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_app_users_ApplicationUserId",
                table: "posts",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_posts_categories_categoryId",
                table: "posts",
                column: "categoryId",
                principalTable: "categories",
                principalColumn: "Id",
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
                name: "FK_user_configs_app_users_ApplicationUserId",
                table: "user_configs",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_metric_app_users_ApplicationUserId",
                table: "user_metric",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_preferences_app_users_ApplicationUserId",
                table: "user_preferences",
                column: "ApplicationUserId",
                principalTable: "app_users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
