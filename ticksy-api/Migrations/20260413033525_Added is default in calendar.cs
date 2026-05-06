using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class Addedisdefaultincalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_default",
                table: "holiday_calendars",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_default",
                table: "holiday_calendars");
        }
    }
}
