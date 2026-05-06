using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class ChangedLeaveRequestStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_time_off_policies_time_off_policy_id",
                table: "leave_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_users_approved_by",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_time_off_policy_id",
                table: "leave_requests");

            migrationBuilder.DropColumn(
                name: "time_off_policy_id",
                table: "leave_requests");

            migrationBuilder.RenameColumn(
                name: "approved_by",
                table: "leave_requests",
                newName: "reviewed_by");

            migrationBuilder.RenameColumn(
                name: "approved_at",
                table: "leave_requests",
                newName: "reviewed_at");

            migrationBuilder.RenameIndex(
                name: "IX_leave_requests_approved_by",
                table: "leave_requests",
                newName: "IX_leave_requests_reviewed_by");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_leave_type_id",
                table: "leave_requests",
                column: "leave_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_requests_time_off_policies_leave_type_id",
                table: "leave_requests",
                column: "leave_type_id",
                principalTable: "time_off_policies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_leave_requests_users_reviewed_by",
                table: "leave_requests",
                column: "reviewed_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_time_off_policies_leave_type_id",
                table: "leave_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_leave_requests_users_reviewed_by",
                table: "leave_requests");

            migrationBuilder.DropIndex(
                name: "IX_leave_requests_leave_type_id",
                table: "leave_requests");

            migrationBuilder.RenameColumn(
                name: "reviewed_by",
                table: "leave_requests",
                newName: "approved_by");

            migrationBuilder.RenameColumn(
                name: "reviewed_at",
                table: "leave_requests",
                newName: "approved_at");

            migrationBuilder.RenameIndex(
                name: "IX_leave_requests_reviewed_by",
                table: "leave_requests",
                newName: "IX_leave_requests_approved_by");

            migrationBuilder.AddColumn<int>(
                name: "time_off_policy_id",
                table: "leave_requests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_time_off_policy_id",
                table: "leave_requests",
                column: "time_off_policy_id");

            migrationBuilder.AddForeignKey(
                name: "FK_leave_requests_time_off_policies_time_off_policy_id",
                table: "leave_requests",
                column: "time_off_policy_id",
                principalTable: "time_off_policies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_leave_requests_users_approved_by",
                table: "leave_requests",
                column: "approved_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
