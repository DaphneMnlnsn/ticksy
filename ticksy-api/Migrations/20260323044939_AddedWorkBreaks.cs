using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class AddedWorkBreaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "break_duration",
                table: "work_schedule_days");

            migrationBuilder.DropColumn(
                name: "active_from",
                table: "user_work_schedules");

            migrationBuilder.DropColumn(
                name: "active_to",
                table: "user_work_schedules");

            migrationBuilder.DropColumn(
                name: "custom_break_duration",
                table: "user_work_schedules");

            migrationBuilder.DropColumn(
                name: "custom_end_time",
                table: "user_work_schedules");

            migrationBuilder.DropColumn(
                name: "custom_start_time",
                table: "user_work_schedules");

            migrationBuilder.CreateTable(
                name: "work_schedule_breaks",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    work_schedule_day_id = table.Column<int>(type: "integer", nullable: false),
                    break_name = table.Column<string>(type: "text", nullable: true),
                    break_duration = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_schedule_breaks", x => x.id);
                    table.ForeignKey(
                        name: "FK_work_schedule_breaks_work_schedule_days_work_schedule_day_id",
                        column: x => x.work_schedule_day_id,
                        principalTable: "work_schedule_days",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_work_schedule_breaks_work_schedule_day_id",
                table: "work_schedule_breaks",
                column: "work_schedule_day_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_schedule_breaks");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "break_duration",
                table: "work_schedule_days",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "active_from",
                table: "user_work_schedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "active_to",
                table: "user_work_schedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "custom_break_duration",
                table: "user_work_schedules",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "custom_end_time",
                table: "user_work_schedules",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "custom_start_time",
                table: "user_work_schedules",
                type: "time without time zone",
                nullable: true);
        }
    }
}
