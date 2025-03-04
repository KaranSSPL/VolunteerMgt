using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerMgt.Server.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Defaulttime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "VolunteerServiceMapping",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "CreatedDate",
                table: "VolunteerServiceMapping",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
