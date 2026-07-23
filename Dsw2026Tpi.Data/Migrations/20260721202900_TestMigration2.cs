using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dsw2026Tpi.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestMigration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_Date_StartTime",
                table: "AvailabilitySlots");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_DoctorId_Date_StartTime",
                table: "AvailabilitySlots",
                columns: new[] { "DoctorId", "Date", "StartTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySlots_DoctorId_Date_StartTime",
                table: "AvailabilitySlots");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySlots_Date_StartTime",
                table: "AvailabilitySlots",
                columns: new[] { "Date", "StartTime" });
        }
    }
}
