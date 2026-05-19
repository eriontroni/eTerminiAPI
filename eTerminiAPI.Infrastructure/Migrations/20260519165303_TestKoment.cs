using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eTerminiAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestKoment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Coment",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coment",
                table: "Appointments");
        }
    }
}
