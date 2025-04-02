using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerMgt.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrectDefaulttime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefualtTime",
                table: "Service",
                newName: "DefaultTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DefaultTime",
                table: "Service",
                newName: "DefualtTime");
        }
    }
}
