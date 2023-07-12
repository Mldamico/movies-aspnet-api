using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Movies.Migrations
{
    /// <inheritdoc />
    public partial class CinemaAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "Address",
                table: "Cinemas",
                type: "geometry",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d7a2855a-b5f3-4b1d-9df0-b05805bea60c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29d1217d-2f24-4a9c-a7a4-a1e19ef44a02", "AQAAAAIAAYagAAAAEM36Al4Il+k5j9RXXmSgcPlaX9yMmh/LNcf1KOSNKOcB7dtVp2xrYG10Hnx3i+W5Qg==", "0e09ac66-28a2-429e-848a-abd98a9695fc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Cinemas");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d7a2855a-b5f3-4b1d-9df0-b05805bea60c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "36556380-2748-4639-a3f8-3c9edb56954a", "AQAAAAIAAYagAAAAEAW4oEPDUgliFLzsT+ocfT8dlzG46PFaPzMF4aXI8G+qXH1VIpNNBnys9Xau4t1Icw==", "ac158473-9127-4d5d-913b-0ebfae03f7fa" });
        }
    }
}
