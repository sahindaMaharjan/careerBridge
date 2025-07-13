using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace careerBridge.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationAndSalaryToJobListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Location",
                table: "JobListings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Salary",
                table: "JobListings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "JobListings");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "JobListings");
        }
    }
}
