using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Movies.Migrations
{
    /// <inheritdoc />
    public partial class CinemaData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d7a2855a-b5f3-4b1d-9df0-b05805bea60c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "712fd8dc-6edf-4ef6-8e09-d28b7992cc4b", "AQAAAAIAAYagAAAAEHTJO25sc+QZ+3uMDzUNcgiPigh2ah6TH/5045zZvnk8juqxWQxJmx/vIQKUqyulcw==", "e945d10b-185f-4b27-adf6-05909da9c8e9" });

            migrationBuilder.InsertData(
                table: "Cinemas",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[,]
                {
                    { 4, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-58.5257443 -34.5086035)"), "Unicenter" },
                    { 5, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-58.5835387 -34.6390319)"), "Al Oeste" },
                    { 6, (NetTopologySuite.Geometries.Point)new NetTopologySuite.IO.WKTReader().Read("SRID=4326;POINT (-73.986227 40.730898)"), "Village East Cinema" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cinemas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d7a2855a-b5f3-4b1d-9df0-b05805bea60c",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29d1217d-2f24-4a9c-a7a4-a1e19ef44a02", "AQAAAAIAAYagAAAAEM36Al4Il+k5j9RXXmSgcPlaX9yMmh/LNcf1KOSNKOcB7dtVp2xrYG10Hnx3i+W5Qg==", "0e09ac66-28a2-429e-848a-abd98a9695fc" });
        }
    }
}
