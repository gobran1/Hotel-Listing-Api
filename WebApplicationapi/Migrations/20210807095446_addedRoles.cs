using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplicationapi.Migrations
{
    public partial class addedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "020ac0a5-8693-4d66-9d52-444b42f95340", "f497b8e6-5dae-4d51-a76a-a7e3551f8ddb", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "711147c9-d96a-40b7-8ad0-22d6db3d1e0a", "bf7a724c-aa76-4eb3-8d7a-9e645040adc5", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "020ac0a5-8693-4d66-9d52-444b42f95340");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "711147c9-d96a-40b7-8ad0-22d6db3d1e0a");
        }
    }
}
