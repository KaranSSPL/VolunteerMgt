using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerMgt.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class VolunteerCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Coupon",
                table: "VolunteerServiceMapping",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Coupon",
                table: "VolunteerServiceMapping");
        }
    }
}
