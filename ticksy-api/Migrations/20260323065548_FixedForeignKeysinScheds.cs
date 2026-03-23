using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class FixedForeignKeysinScheds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_users_user_id",
                table: "team_invites");

            migrationBuilder.DropIndex(
                name: "IX_team_invites_user_id",
                table: "team_invites");

            migrationBuilder.DropColumn(
                name: "schedule_id",
                table: "user_work_schedules");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "team_invites");

            migrationBuilder.AlterColumn<string>(
                name: "break_name",
                table: "work_schedule_breaks",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_created_by",
                table: "team_invites",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_users_created_by",
                table: "team_invites",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_users_created_by",
                table: "team_invites");

            migrationBuilder.DropIndex(
                name: "IX_team_invites_created_by",
                table: "team_invites");

            migrationBuilder.AlterColumn<string>(
                name: "break_name",
                table: "work_schedule_breaks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "schedule_id",
                table: "user_work_schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "team_invites",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_user_id",
                table: "team_invites",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_users_user_id",
                table: "team_invites",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
