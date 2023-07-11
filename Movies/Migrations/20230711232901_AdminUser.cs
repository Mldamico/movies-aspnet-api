using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movies.Migrations
{
    /// <inheritdoc />
    public partial class AdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "bcd2b5b5-6335-4639-8f99-b0f9126f92ed", null, "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d7a2855a-b5f3-4b1d-9df0-b05805bea60c", 0, "36556380-2748-4639-a3f8-3c9edb56954a", "admin@gmail.com", false, false, null, "admin@gmail.com", "adminUser", "AQAAAAIAAYagAAAAEAW4oEPDUgliFLzsT+ocfT8dlzG46PFaPzMF4aXI8G+qXH1VIpNNBnys9Xau4t1Icw==", null, false, "ac158473-9127-4d5d-913b-0ebfae03f7fa", false, "adminUser" });

            migrationBuilder.InsertData(
                table: "AspNetUserClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "UserId" },
                values: new object[] { 1, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin", "d7a2855a-b5f3-4b1d-9df0-b05805bea60c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bcd2b5b5-6335-4639-8f99-b0f9126f92ed");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d7a2855a-b5f3-4b1d-9df0-b05805bea60c");
        }
    }
}
