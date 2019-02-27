using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class AppoinmentForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StaffAtendee",
                table: "Appointments",
                newName: "StaffMemberId");

            migrationBuilder.AddColumn<Guid>(
                name: "AppointLocationId",
                table: "Appointments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AppointTypeId",
                table: "Appointments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointLocationId",
                table: "Appointments",
                column: "AppointLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointTypeId",
                table: "Appointments",
                column: "AppointTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerAccountId",
                table: "Appointments",
                column: "CustomerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StaffMemberId",
                table: "Appointments",
                column: "StaffMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentLocations_AppointLocationId",
                table: "Appointments",
                column: "AppointLocationId",
                principalTable: "AppointmentLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentTypes_AppointTypeId",
                table: "Appointments",
                column: "AppointTypeId",
                principalTable: "AppointmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_CustomerAccounts_CustomerAccountId",
                table: "Appointments",
                column: "CustomerAccountId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_StaffMembers_StaffMemberId",
                table: "Appointments",
                column: "StaffMemberId",
                principalTable: "StaffMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentLocations_AppointLocationId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentTypes_AppointTypeId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_CustomerAccounts_CustomerAccountId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_StaffMembers_StaffMemberId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointLocationId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointTypeId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CustomerAccountId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_StaffMemberId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointLocationId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointTypeId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "StaffMemberId",
                table: "Appointments",
                newName: "StaffAtendee");
        }
    }
}
