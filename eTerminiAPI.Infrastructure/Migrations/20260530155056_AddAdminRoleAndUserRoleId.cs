using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eTerminiAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminRoleAndUserRoleId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminRoleId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRoles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_AdminRoleId",
                table: "Users",
                column: "AdminRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AdminRoles_AdminRoleId",
                table: "Users",
                column: "AdminRoleId",
                principalTable: "AdminRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AdminRoles_AdminRoleId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AdminRoles");

            migrationBuilder.DropIndex(
                name: "IX_Users_AdminRoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdminRoleId",
                table: "Users");
        }
    }
}
