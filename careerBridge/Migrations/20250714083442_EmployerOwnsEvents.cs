using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace careerBridge.Migrations
{
    /// <inheritdoc />
    public partial class EmployerOwnsEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Employers_EmployerProfileEmployerID",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Mentors_MentorID",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "MentorID",
                table: "Events",
                newName: "EmployerID");

            migrationBuilder.RenameColumn(
                name: "EmployerProfileEmployerID",
                table: "Events",
                newName: "MentorProfileMentorID");

            migrationBuilder.RenameIndex(
                name: "IX_Events_MentorID",
                table: "Events",
                newName: "IX_Events_EmployerID");

            migrationBuilder.RenameIndex(
                name: "IX_Events_EmployerProfileEmployerID",
                table: "Events",
                newName: "IX_Events_MentorProfileMentorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Employers_EmployerID",
                table: "Events",
                column: "EmployerID",
                principalTable: "Employers",
                principalColumn: "EmployerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Mentors_MentorProfileMentorID",
                table: "Events",
                column: "MentorProfileMentorID",
                principalTable: "Mentors",
                principalColumn: "MentorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Employers_EmployerID",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Mentors_MentorProfileMentorID",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "MentorProfileMentorID",
                table: "Events",
                newName: "EmployerProfileEmployerID");

            migrationBuilder.RenameColumn(
                name: "EmployerID",
                table: "Events",
                newName: "MentorID");

            migrationBuilder.RenameIndex(
                name: "IX_Events_MentorProfileMentorID",
                table: "Events",
                newName: "IX_Events_EmployerProfileEmployerID");

            migrationBuilder.RenameIndex(
                name: "IX_Events_EmployerID",
                table: "Events",
                newName: "IX_Events_MentorID");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Employers_EmployerProfileEmployerID",
                table: "Events",
                column: "EmployerProfileEmployerID",
                principalTable: "Employers",
                principalColumn: "EmployerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Mentors_MentorID",
                table: "Events",
                column: "MentorID",
                principalTable: "Mentors",
                principalColumn: "MentorID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
