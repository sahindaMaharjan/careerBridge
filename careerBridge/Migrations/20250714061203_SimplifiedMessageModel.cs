using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace careerBridge.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedMessageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Employers_ReceiverEmployerID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Employers_SenderEmployerID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Mentors_ReceiverMentorID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Mentors_SenderMentorID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Students_ReceiverStudentID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Students_SenderStudentID",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderStudentID",
                table: "Messages",
                newName: "StudentProfileStudentID1");

            migrationBuilder.RenameColumn(
                name: "SenderMentorID",
                table: "Messages",
                newName: "StudentProfileStudentID");

            migrationBuilder.RenameColumn(
                name: "SenderEmployerID",
                table: "Messages",
                newName: "MentorProfileMentorID1");

            migrationBuilder.RenameColumn(
                name: "ReceiverStudentID",
                table: "Messages",
                newName: "MentorProfileMentorID");

            migrationBuilder.RenameColumn(
                name: "ReceiverMentorID",
                table: "Messages",
                newName: "EmployerProfileEmployerID1");

            migrationBuilder.RenameColumn(
                name: "ReceiverEmployerID",
                table: "Messages",
                newName: "EmployerProfileEmployerID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderStudentID",
                table: "Messages",
                newName: "IX_Messages_StudentProfileStudentID1");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderMentorID",
                table: "Messages",
                newName: "IX_Messages_StudentProfileStudentID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderEmployerID",
                table: "Messages",
                newName: "IX_Messages_MentorProfileMentorID1");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReceiverStudentID",
                table: "Messages",
                newName: "IX_Messages_MentorProfileMentorID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReceiverMentorID",
                table: "Messages",
                newName: "IX_Messages_EmployerProfileEmployerID1");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ReceiverEmployerID",
                table: "Messages",
                newName: "IX_Messages_EmployerProfileEmployerID");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverUserID",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderUserID",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReceiverUserID",
                table: "Messages",
                column: "ReceiverUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderUserID",
                table: "Messages",
                column: "SenderUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverUserID",
                table: "Messages",
                column: "ReceiverUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderUserID",
                table: "Messages",
                column: "SenderUserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Employers_EmployerProfileEmployerID",
                table: "Messages",
                column: "EmployerProfileEmployerID",
                principalTable: "Employers",
                principalColumn: "EmployerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Employers_EmployerProfileEmployerID1",
                table: "Messages",
                column: "EmployerProfileEmployerID1",
                principalTable: "Employers",
                principalColumn: "EmployerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Mentors_MentorProfileMentorID",
                table: "Messages",
                column: "MentorProfileMentorID",
                principalTable: "Mentors",
                principalColumn: "MentorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Mentors_MentorProfileMentorID1",
                table: "Messages",
                column: "MentorProfileMentorID1",
                principalTable: "Mentors",
                principalColumn: "MentorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Students_StudentProfileStudentID",
                table: "Messages",
                column: "StudentProfileStudentID",
                principalTable: "Students",
                principalColumn: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Students_StudentProfileStudentID1",
                table: "Messages",
                column: "StudentProfileStudentID1",
                principalTable: "Students",
                principalColumn: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverUserID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderUserID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Employers_EmployerProfileEmployerID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Employers_EmployerProfileEmployerID1",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Mentors_MentorProfileMentorID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Mentors_MentorProfileMentorID1",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Students_StudentProfileStudentID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Students_StudentProfileStudentID1",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReceiverUserID",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_SenderUserID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ReceiverUserID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SenderUserID",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "StudentProfileStudentID1",
                table: "Messages",
                newName: "SenderStudentID");

            migrationBuilder.RenameColumn(
                name: "StudentProfileStudentID",
                table: "Messages",
                newName: "SenderMentorID");

            migrationBuilder.RenameColumn(
                name: "MentorProfileMentorID1",
                table: "Messages",
                newName: "SenderEmployerID");

            migrationBuilder.RenameColumn(
                name: "MentorProfileMentorID",
                table: "Messages",
                newName: "ReceiverStudentID");

            migrationBuilder.RenameColumn(
                name: "EmployerProfileEmployerID1",
                table: "Messages",
                newName: "ReceiverMentorID");

            migrationBuilder.RenameColumn(
                name: "EmployerProfileEmployerID",
                table: "Messages",
                newName: "ReceiverEmployerID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_StudentProfileStudentID1",
                table: "Messages",
                newName: "IX_Messages_SenderStudentID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_StudentProfileStudentID",
                table: "Messages",
                newName: "IX_Messages_SenderMentorID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_MentorProfileMentorID1",
                table: "Messages",
                newName: "IX_Messages_SenderEmployerID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_MentorProfileMentorID",
                table: "Messages",
                newName: "IX_Messages_ReceiverStudentID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_EmployerProfileEmployerID1",
                table: "Messages",
                newName: "IX_Messages_ReceiverMentorID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_EmployerProfileEmployerID",
                table: "Messages",
                newName: "IX_Messages_ReceiverEmployerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Employers_ReceiverEmployerID",
                table: "Messages",
                column: "ReceiverEmployerID",
                principalTable: "Employers",
                principalColumn: "EmployerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Employers_SenderEmployerID",
                table: "Messages",
                column: "SenderEmployerID",
                principalTable: "Employers",
                principalColumn: "EmployerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Mentors_ReceiverMentorID",
                table: "Messages",
                column: "ReceiverMentorID",
                principalTable: "Mentors",
                principalColumn: "MentorID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Mentors_SenderMentorID",
                table: "Messages",
                column: "SenderMentorID",
                principalTable: "Mentors",
                principalColumn: "MentorID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Students_ReceiverStudentID",
                table: "Messages",
                column: "ReceiverStudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Students_SenderStudentID",
                table: "Messages",
                column: "SenderStudentID",
                principalTable: "Students",
                principalColumn: "StudentID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
