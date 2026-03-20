using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraFks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_holidays_users_created_by_user_id",
                table: "holidays");

            migrationBuilder.DropForeignKey(
                name: "FK_reports_users_generated_by_user_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_teams_team_id",
                table: "team_invites");

            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_users_created_by_user_id",
                table: "team_invites");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_users_created_by_user_id",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "FK_work_schedules_users_created_by_user_id",
                table: "work_schedules");

            migrationBuilder.DropIndex(
                name: "IX_work_schedules_created_by_user_id",
                table: "work_schedules");

            migrationBuilder.DropIndex(
                name: "IX_teams_created_by_user_id",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "IX_team_invites_created_by_user_id",
                table: "team_invites");

            migrationBuilder.DropIndex(
                name: "IX_reports_generated_by_user_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_holidays_created_by_user_id",
                table: "holidays");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "work_schedules");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "team_invites");

            migrationBuilder.DropColumn(
                name: "generated_by_user_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "created_by_user_id",
                table: "holidays");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "team_invites",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_schedules_created_by",
                table: "work_schedules",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_teams_created_by",
                table: "teams",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_user_id",
                table: "team_invites",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_generated_by",
                table: "reports",
                column: "generated_by");

            migrationBuilder.CreateIndex(
                name: "IX_holidays_created_by",
                table: "holidays",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_holidays_users_created_by",
                table: "holidays",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reports_users_generated_by",
                table: "reports",
                column: "generated_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_users_user_id",
                table: "team_invites",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_teams_users_created_by",
                table: "teams",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_work_schedules_users_created_by",
                table: "work_schedules",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_holidays_users_created_by",
                table: "holidays");

            migrationBuilder.DropForeignKey(
                name: "FK_reports_users_generated_by",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "FK_team_invites_users_user_id",
                table: "team_invites");

            migrationBuilder.DropForeignKey(
                name: "FK_teams_users_created_by",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "FK_work_schedules_users_created_by",
                table: "work_schedules");

            migrationBuilder.DropIndex(
                name: "IX_work_schedules_created_by",
                table: "work_schedules");

            migrationBuilder.DropIndex(
                name: "IX_teams_created_by",
                table: "teams");

            migrationBuilder.DropIndex(
                name: "IX_team_invites_user_id",
                table: "team_invites");

            migrationBuilder.DropIndex(
                name: "IX_reports_generated_by",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "IX_holidays_created_by",
                table: "holidays");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "team_invites");

            migrationBuilder.AddColumn<int>(
                name: "created_by_user_id",
                table: "work_schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by_user_id",
                table: "teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by_user_id",
                table: "team_invites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "generated_by_user_id",
                table: "reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by_user_id",
                table: "holidays",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_work_schedules_created_by_user_id",
                table: "work_schedules",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_teams_created_by_user_id",
                table: "teams",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_created_by_user_id",
                table: "team_invites",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_reports_generated_by_user_id",
                table: "reports",
                column: "generated_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_holidays_created_by_user_id",
                table: "holidays",
                column: "created_by_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_holidays_users_created_by_user_id",
                table: "holidays",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reports_users_generated_by_user_id",
                table: "reports",
                column: "generated_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_teams_team_id",
                table: "team_invites",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_team_invites_users_created_by_user_id",
                table: "team_invites",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_users_created_by_user_id",
                table: "teams",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_work_schedules_users_created_by_user_id",
                table: "work_schedules",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
