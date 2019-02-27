using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class BusinessLeadManagerAsStaffMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Manager",
                table: "BusinessLeads",
                newName: "StaffMember.Id");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLeads_StaffMember.Id",
                table: "BusinessLeads",
                column: "StaffMember.Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessLeads_StaffMembers_StaffMember.Id",
                table: "BusinessLeads",
                column: "StaffMember.Id",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessLeads_StaffMembers_StaffMember.Id",
                table: "BusinessLeads");

            migrationBuilder.DropIndex(
                name: "IX_BusinessLeads_StaffMember.Id",
                table: "BusinessLeads");

            migrationBuilder.RenameColumn(
                name: "StaffMember.Id",
                table: "BusinessLeads",
                newName: "Manager");
        }
    }
}
