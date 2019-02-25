using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class StaffMemberPositionAndDepartmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_DepartmentId",
                table: "StaffMembers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_StaffPositionId",
                table: "StaffMembers",
                column: "StaffPositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffMembers_Departments_DepartmentId",
                table: "StaffMembers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffMembers_StaffPositions_StaffPositionId",
                table: "StaffMembers",
                column: "StaffPositionId",
                principalTable: "StaffPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffMembers_Departments_DepartmentId",
                table: "StaffMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffMembers_StaffPositions_StaffPositionId",
                table: "StaffMembers");

            migrationBuilder.DropIndex(
                name: "IX_StaffMembers_DepartmentId",
                table: "StaffMembers");

            migrationBuilder.DropIndex(
                name: "IX_StaffMembers_StaffPositionId",
                table: "StaffMembers");
        }
    }
}
