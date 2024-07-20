using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerReport.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVolunteerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Volunteers");
        }
    }
}
