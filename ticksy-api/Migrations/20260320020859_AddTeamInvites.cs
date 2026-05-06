using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ticksy_api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamInvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_teams_join_code",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "join_code",
                table: "teams");

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "teams",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "team_invites",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    team_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false),
                    created_by_user_id = table.Column<int>(type: "integer", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_invites", x => x.id);
                    table.ForeignKey(
                        name: "FK_team_invites_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_team_invites_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_teams_team_id",
                table: "teams",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_created_by_user_id",
                table: "team_invites",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_expires_at",
                table: "team_invites",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_team_id",
                table: "team_invites",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_invites_token",
                table: "team_invites",
                column: "token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_teams_teams_team_id",
                table: "teams",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_teams_teams_team_id",
                table: "teams");

            migrationBuilder.DropTable(
                name: "team_invites");

            migrationBuilder.DropIndex(
                name: "IX_teams_team_id",
                table: "teams");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "teams");

            migrationBuilder.AddColumn<string>(
                name: "join_code",
                table: "teams",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_teams_join_code",
                table: "teams",
                column: "join_code",
                unique: true);
        }
    }
}
