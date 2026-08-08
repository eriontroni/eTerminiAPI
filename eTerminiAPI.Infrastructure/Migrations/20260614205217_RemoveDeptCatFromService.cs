using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eTerminiAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeptCatFromService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublicServices_Departments_DepartmentId",
                table: "PublicServices");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicServices_ServiceCategories_CategoryId",
                table: "PublicServices");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "PublicServices",
                newName: "InstitutionId");

            migrationBuilder.RenameIndex(
                name: "IX_PublicServices_CategoryId",
                table: "PublicServices",
                newName: "IX_PublicServices_InstitutionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "PublicServices",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceCategoryId",
                table: "PublicServices",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicServices_ServiceCategoryId",
                table: "PublicServices",
                column: "ServiceCategoryId");

            // Clear stale rows whose InstitutionId values came from ServiceCategories
            // and are not valid Institution IDs — must delete children first.
            migrationBuilder.Sql("DELETE FROM AppointmentStatusHistories WHERE AppointmentId IN (SELECT Id FROM Appointments WHERE TimeSlotId IN (SELECT Id FROM TimeSlots WHERE ServiceId IN (SELECT Id FROM PublicServices)))");
            migrationBuilder.Sql("DELETE FROM Appointments WHERE TimeSlotId IN (SELECT Id FROM TimeSlots WHERE ServiceId IN (SELECT Id FROM PublicServices))");
            migrationBuilder.Sql("DELETE FROM TimeSlots WHERE ServiceId IN (SELECT Id FROM PublicServices)");
            migrationBuilder.Sql("DELETE FROM ServiceRequirements WHERE ServiceId IN (SELECT Id FROM PublicServices)");
            migrationBuilder.Sql("DELETE FROM PublicServices");

            migrationBuilder.AddForeignKey(
                name: "FK_PublicServices_Departments_DepartmentId",
                table: "PublicServices",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PublicServices_Institutions_InstitutionId",
                table: "PublicServices",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicServices_ServiceCategories_ServiceCategoryId",
                table: "PublicServices",
                column: "ServiceCategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PublicServices_Departments_DepartmentId",
                table: "PublicServices");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicServices_Institutions_InstitutionId",
                table: "PublicServices");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicServices_ServiceCategories_ServiceCategoryId",
                table: "PublicServices");

            migrationBuilder.DropIndex(
                name: "IX_PublicServices_ServiceCategoryId",
                table: "PublicServices");

            migrationBuilder.DropColumn(
                name: "ServiceCategoryId",
                table: "PublicServices");

            migrationBuilder.RenameColumn(
                name: "InstitutionId",
                table: "PublicServices",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_PublicServices_InstitutionId",
                table: "PublicServices",
                newName: "IX_PublicServices_CategoryId");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "PublicServices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicServices_Departments_DepartmentId",
                table: "PublicServices",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicServices_ServiceCategories_CategoryId",
                table: "PublicServices",
                column: "CategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
