using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class BusinessLeadLeadManagerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessLeads_StaffMembers_StaffMember.Id",
                table: "BusinessLeads");

            migrationBuilder.DropIndex(
                name: "IX_BusinessLeads_StaffMember.Id",
                table: "BusinessLeads");

            migrationBuilder.DropColumn(
                name: "StaffMember.Id",
                table: "BusinessLeads");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLeads_StaffMemberId",
                table: "BusinessLeads",
                column: "StaffMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusinessLeads_StaffMembers_StaffMemberId",
                table: "BusinessLeads",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusinessLeads_StaffMembers_StaffMemberId",
                table: "BusinessLeads");

            migrationBuilder.DropIndex(
                name: "IX_BusinessLeads_StaffMemberId",
                table: "BusinessLeads");

            migrationBuilder.AddColumn<Guid>(
                name: "StaffMember.Id",
                table: "BusinessLeads",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
    }
}
