using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class AppoinmentLocationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentLocations_AppointLocationId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointLocationId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppointLocationId",
                table: "Appointments");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentLocationId",
                table: "Appointments",
                column: "AppointmentLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentLocations_AppointmentLocationId",
                table: "Appointments",
                column: "AppointmentLocationId",
                principalTable: "AppointmentLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AppointmentLocations_AppointmentLocationId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppointmentLocationId",
                table: "Appointments");

            migrationBuilder.AddColumn<Guid>(
                name: "AppointLocationId",
                table: "Appointments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointLocationId",
                table: "Appointments",
                column: "AppointLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AppointmentLocations_AppointLocationId",
                table: "Appointments",
                column: "AppointLocationId",
                principalTable: "AppointmentLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
