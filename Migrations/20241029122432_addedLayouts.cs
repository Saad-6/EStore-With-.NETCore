using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EStore.Migrations
{
    /// <inheritdoc />
    public partial class addedLayouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HomePageSettingsId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomePageSettingsId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HomePageSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePageSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerReview",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomePageSettingsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerReview_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HeroCarouselSlide",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subtitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ButtonText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomePageSettingsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroCarouselSlide", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroCarouselSlide_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HomePageLayouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomePageLayouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomePageLayouts_HomePageSettings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotDeal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomePageSettingsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotDeal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotDeal_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SeasonalCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomePageSettingsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonalCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonalCollection_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_HomePageSettingsId",
                table: "Products",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_HomePageSettingsId",
                table: "Categories",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerReview_HomePageSettingsId",
                table: "CustomerReview",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroCarouselSlide_HomePageSettingsId",
                table: "HeroCarouselSlide",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_HomePageLayouts_SettingsId",
                table: "HomePageLayouts",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_HotDeal_HomePageSettingsId",
                table: "HotDeal",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonalCollection_HomePageSettingsId",
                table: "SeasonalCollection",
                column: "HomePageSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_HomePageSettings_HomePageSettingsId",
                table: "Categories",
                column: "HomePageSettingsId",
                principalTable: "HomePageSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_HomePageSettings_HomePageSettingsId",
                table: "Products",
                column: "HomePageSettingsId",
                principalTable: "HomePageSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_HomePageSettings_HomePageSettingsId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_HomePageSettings_HomePageSettingsId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "CustomerReview");

            migrationBuilder.DropTable(
                name: "HeroCarouselSlide");

            migrationBuilder.DropTable(
                name: "HomePageLayouts");

            migrationBuilder.DropTable(
                name: "HotDeal");

            migrationBuilder.DropTable(
                name: "SeasonalCollection");

            migrationBuilder.DropTable(
                name: "HomePageSettings");

            migrationBuilder.DropIndex(
                name: "IX_Products_HomePageSettingsId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Categories_HomePageSettingsId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "HomePageSettingsId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HomePageSettingsId",
                table: "Categories");
        }
    }
}
