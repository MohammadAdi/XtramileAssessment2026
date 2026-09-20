using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace XtramileWeather.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCountryCitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", fixedLength: true, maxLength: 2, nullable: false),
                    Latitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: false),
                    Longitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Code", "Name" },
                values: new object[,]
                {
                    { "AU", "Australia" },
                    { "ID", "Indonesia" },
                    { "MY", "Malaysia" },
                    { "SG", "Singapore" },
                    { "US", "United States" }
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryCode", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "AU", -33.8688m, 151.2093m, "Sydney" },
                    { 2, "AU", -37.8136m, 144.9631m, "Melbourne" },
                    { 3, "AU", -27.4698m, 153.0251m, "Brisbane" },
                    { 4, "ID", -6.2088m, 106.8456m, "Jakarta" },
                    { 5, "ID", -7.2575m, 112.7521m, "Surabaya" },
                    { 6, "ID", -6.9175m, 107.6191m, "Bandung" },
                    { 7, "MY", 3.1390m, 101.6869m, "Kuala Lumpur" },
                    { 8, "MY", 5.4141m, 100.3288m, "George Town" },
                    { 9, "MY", 1.4927m, 103.7414m, "Johor Bahru" },
                    { 10, "SG", 1.3521m, 103.8198m, "Singapore" },
                    { 11, "US", 40.7128m, -74.0060m, "New York" },
                    { 12, "US", 34.0522m, -118.2437m, "Los Angeles" },
                    { 13, "US", 41.8781m, -87.6298m, "Chicago" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryCode_Name",
                table: "Cities",
                columns: new[] { "CountryCode", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
