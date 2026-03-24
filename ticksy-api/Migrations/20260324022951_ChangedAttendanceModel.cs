using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAttendanceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "manually_entered",
                table: "attendances");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "break_duration",
                table: "attendances",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "interval",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "break_start",
                table: "attendances",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "attendances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_work_minutes",
                table: "attendances",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "break_start",
                table: "attendances");

            migrationBuilder.DropColumn(
                name: "status",
                table: "attendances");

            migrationBuilder.DropColumn(
                name: "total_work_minutes",
                table: "attendances");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "break_duration",
                table: "attendances",
                type: "interval",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AddColumn<bool>(
                name: "manually_entered",
                table: "attendances",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
