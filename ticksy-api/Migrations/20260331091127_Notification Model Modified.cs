using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class NotificationModelModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "notifications");
        }
    }
}
