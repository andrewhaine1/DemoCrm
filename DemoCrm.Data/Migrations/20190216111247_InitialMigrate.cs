using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class InitialMigrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Subject = table.Column<string>(maxLength: 150, nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    ContactPersonFullName = table.Column<string>(maxLength: 150, nullable: false),
                    AppointmentLocationId = table.Column<Guid>(nullable: false),
                    AppointmentTypeId = table.Column<Guid>(nullable: false),
                    AppointmentAddress = table.Column<string>(maxLength: 500, nullable: true),
                    IsCompleted = table.Column<bool>(nullable: false),
                    StaffAtendee = table.Column<Guid>(nullable: false),
                    CustomerAccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessLeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    Company = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(maxLength: 150, nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: false),
                    Manager = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLeads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmObjectTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmObjectTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompanyName = table.Column<string>(maxLength: 150, nullable: false),
                    Address = table.Column<string>(maxLength: 150, nullable: false),
                    RegistrationNumber = table.Column<string>(maxLength: 150, nullable: false),
                    VatNumber = table.Column<string>(maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 150, nullable: false),
                    AccountOwner = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    Department = table.Column<string>(maxLength: 150, nullable: false),
                    Position = table.Column<string>(maxLength: 150, nullable: true),
                    Email = table.Column<string>(maxLength: 150, nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: false),
                    CustomerAccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    Manager = table.Column<Guid>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    StaffPrositionId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: false),
                    IsManager = table.Column<bool>(nullable: false),
                    DepartmentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrmUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OauthId = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 150, nullable: false),
                    LastName = table.Column<string>(maxLength: 150, nullable: false),
                    Email = table.Column<string>(maxLength: 150, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 150, nullable: false),
                    ObjectTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrmUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrmUsers_CrmObjectTypes_ObjectTypeId",
                        column: x => x.ObjectTypeId,
                        principalTable: "CrmObjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 150, nullable: false),
                    Address = table.Column<string>(maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 20, nullable: false),
                    CrmUserId = table.Column<Guid>(nullable: false),
                    ObjectTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_CrmUsers_CrmUserId",
                        column: x => x.CrmUserId,
                        principalTable: "CrmUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Companies_CrmObjectTypes_ObjectTypeId",
                        column: x => x.ObjectTypeId,
                        principalTable: "CrmObjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CrmUserId",
                table: "Companies",
                column: "CrmUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ObjectTypeId",
                table: "Companies",
                column: "ObjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CrmUsers_OauthId",
                table: "CrmUsers",
                column: "OauthId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CrmUsers_ObjectTypeId",
                table: "CrmUsers",
                column: "ObjectTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentLocations");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentTypes");

            migrationBuilder.DropTable(
                name: "BusinessLeads");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "CustomerAccounts");

            migrationBuilder.DropTable(
                name: "CustomerContacts");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "StaffMembers");

            migrationBuilder.DropTable(
                name: "StaffPositions");

            migrationBuilder.DropTable(
                name: "CrmUsers");

            migrationBuilder.DropTable(
                name: "CrmObjectTypes");
        }
    }
}
