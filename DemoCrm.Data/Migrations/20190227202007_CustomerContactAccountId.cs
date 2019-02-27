using Microsoft.EntityFrameworkCore.Migrations;

namespace DemoCrm.Data.Migrations
{
    public partial class CustomerContactAccountId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CustomerContacts_CustomerAccountId",
                table: "CustomerContacts",
                column: "CustomerAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerContacts_CustomerAccounts_CustomerAccountId",
                table: "CustomerContacts",
                column: "CustomerAccountId",
                principalTable: "CustomerAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerContacts_CustomerAccounts_CustomerAccountId",
                table: "CustomerContacts");

            migrationBuilder.DropIndex(
                name: "IX_CustomerContacts_CustomerAccountId",
                table: "CustomerContacts");
        }
    }
}
