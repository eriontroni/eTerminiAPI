using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eTerminiAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coment",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Coment",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
