using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class AddedHolidayCalendars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "external_id",
                table: "holidays",
                newName: "external_calendar_id");

            migrationBuilder.AddColumn<int>(
                name: "calendar_id",
                table: "holidays",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "holiday_calendars",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    source = table.Column<int>(type: "integer", nullable: false),
                    external_calendar_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_holiday_calendars", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_holidays_calendar_id",
                table: "holidays",
                column: "calendar_id");

            migrationBuilder.AddForeignKey(
                name: "FK_holidays_holiday_calendars_calendar_id",
                table: "holidays",
                column: "calendar_id",
                principalTable: "holiday_calendars",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_holidays_holiday_calendars_calendar_id",
                table: "holidays");

            migrationBuilder.DropTable(
                name: "holiday_calendars");

            migrationBuilder.DropIndex(
                name: "IX_holidays_calendar_id",
                table: "holidays");

            migrationBuilder.DropColumn(
                name: "calendar_id",
                table: "holidays");

            migrationBuilder.RenameColumn(
                name: "external_calendar_id",
                table: "holidays",
                newName: "external_id");
        }
    }
}
