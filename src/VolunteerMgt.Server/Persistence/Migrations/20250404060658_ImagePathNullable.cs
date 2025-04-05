using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerMgt.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImagePathNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availability_Volunteer_VolunteerId",
                table: "Availability");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Volunteer",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "VolunteerId",
                table: "Availability",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TimeSlot",
                table: "Availability",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Day",
                table: "Availability",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Availability_Volunteer_VolunteerId",
                table: "Availability",
                column: "VolunteerId",
                principalTable: "Volunteer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Availability_Volunteer_VolunteerId",
                table: "Availability");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Volunteer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VolunteerId",
                table: "Availability",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TimeSlot",
                table: "Availability",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Day",
                table: "Availability",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Availability_Volunteer_VolunteerId",
                table: "Availability",
                column: "VolunteerId",
                principalTable: "Volunteer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
