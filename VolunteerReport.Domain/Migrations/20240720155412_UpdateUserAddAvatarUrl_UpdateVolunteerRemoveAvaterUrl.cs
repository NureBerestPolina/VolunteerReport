using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerReport.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAddAvatarUrl_UpdateVolunteerRemoveAvaterUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Volunteers");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
