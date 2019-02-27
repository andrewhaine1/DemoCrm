using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class CustomerAccountStaffMemberAndCompanyId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccountOwner",
                table: "CustomerAccounts",
                newName: "StaffMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_CompanyId",
                table: "CustomerAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_StaffMemberId",
                table: "CustomerAccounts",
                column: "StaffMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAccounts_Companies_CompanyId",
                table: "CustomerAccounts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerAccounts_StaffMembers_StaffMemberId",
                table: "CustomerAccounts",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAccounts_Companies_CompanyId",
                table: "CustomerAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerAccounts_StaffMembers_StaffMemberId",
                table: "CustomerAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAccounts_CompanyId",
                table: "CustomerAccounts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerAccounts_StaffMemberId",
                table: "CustomerAccounts");

            migrationBuilder.RenameColumn(
                name: "StaffMemberId",
                table: "CustomerAccounts",
                newName: "AccountOwner");
        }
    }
}
