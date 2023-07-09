using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Movies.Migrations
{
    /// <inheritdoc />
    public partial class Cinema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MoviesGenres",
                table: "MoviesGenres");

            migrationBuilder.DropIndex(
                name: "IX_MoviesGenres_GenreId",
                table: "MoviesGenres");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoviesGenres",
                table: "MoviesGenres",
                columns: new[] { "GenreId", "MovieId" });

            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoviesCinemas",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "integer", nullable: false),
                    CinemaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesCinemas", x => new { x.MovieId, x.CinemaId });
                    table.ForeignKey(
                        name: "FK_MoviesCinemas_Cinemas_CinemaId",
                        column: x => x.CinemaId,
                        principalTable: "Cinemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoviesCinemas_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesGenres_MovieId",
                table: "MoviesGenres",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesCinemas_CinemaId",
                table: "MoviesCinemas",
                column: "CinemaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviesCinemas");

            migrationBuilder.DropTable(
                name: "Cinemas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MoviesGenres",
                table: "MoviesGenres");

            migrationBuilder.DropIndex(
                name: "IX_MoviesGenres_MovieId",
                table: "MoviesGenres");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MoviesGenres",
                table: "MoviesGenres",
                columns: new[] { "MovieId", "GenreId" });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesGenres_GenreId",
                table: "MoviesGenres",
                column: "GenreId");
        }
    }
}
