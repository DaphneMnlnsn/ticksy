using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class Workschedulestructurechanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "weekly_duration",
                table: "work_schedules",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "duration",
                table: "work_schedule_days",
                type: "interval",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "weekly_duration",
                table: "work_schedules");

            migrationBuilder.DropColumn(
                name: "duration",
                table: "work_schedule_days");
        }
    }
}
