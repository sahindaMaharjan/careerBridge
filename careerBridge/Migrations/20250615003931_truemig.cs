using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace careerBridge.Migrations
{
    /// <inheritdoc />
    public partial class truemig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfferLetterPath",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CompanyWebsite",
                table: "Employers");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Mentors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Mentors");

            migrationBuilder.AddColumn<string>(
                name: "OfferLetterPath",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyWebsite",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
