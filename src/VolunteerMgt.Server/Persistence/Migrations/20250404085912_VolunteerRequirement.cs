using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerMgt.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VolunteerRequirement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequiredVolunteer",
                table: "Service",
                newName: "SundayVolunteerRequirement");

            migrationBuilder.AddColumn<string>(
                name: "EkadashiVolunteerRequirement",
                table: "Service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FestivalVolunteerRequirement",
                table: "Service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SaturdayVolunteerRequirement",
                table: "Service",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EkadashiVolunteerRequirement",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "FestivalVolunteerRequirement",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "SaturdayVolunteerRequirement",
                table: "Service");

            migrationBuilder.RenameColumn(
                name: "SundayVolunteerRequirement",
                table: "Service",
                newName: "RequiredVolunteer");
        }
    }
}
