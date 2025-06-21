using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnInUserMetric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "play_list_count",
                table: "user_metric",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_user_metric_play_list_count",
                table: "user_metric",
                column: "play_list_count");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_metric_play_list_count",
                table: "user_metric");

            migrationBuilder.DropColumn(
                name: "play_list_count",
                table: "user_metric");
        }
    }
}
