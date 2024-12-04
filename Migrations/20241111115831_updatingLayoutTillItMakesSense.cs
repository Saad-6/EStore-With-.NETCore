using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EStore.Migrations
{
    /// <inheritdoc />
    public partial class updatingLayoutTillItMakesSense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_HomePageSettings_HomePageSettingsId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_HomePageLayouts_HomePageSettings_SettingsId",
                table: "HomePageLayouts");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_HomePageSettings_HomePageSettingsId",
                table: "Products");

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

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SeasonalCollection",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "SeasonalCollection",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "HotDeal",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "HotDeal",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SettingsId",
                table: "HomePageLayouts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Subtitle",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ButtonText",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "CustomerReview",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CustomerReview",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "CustomerReview",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "CustomerReview",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "SimpleCategoryDTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomePageSettingsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleCategoryDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimpleCategoryDTO_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SimpleProductDTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HomePageSettingsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimpleProductDTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimpleProductDTO_HomePageSettings_HomePageSettingsId",
                        column: x => x.HomePageSettingsId,
                        principalTable: "HomePageSettings",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimpleCategoryDTO_HomePageSettingsId",
                table: "SimpleCategoryDTO",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_SimpleProductDTO_HomePageSettingsId",
                table: "SimpleProductDTO",
                column: "HomePageSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomePageLayouts_HomePageSettings_SettingsId",
                table: "HomePageLayouts",
                column: "SettingsId",
                principalTable: "HomePageSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomePageLayouts_HomePageSettings_SettingsId",
                table: "HomePageLayouts");

            migrationBuilder.DropTable(
                name: "SimpleCategoryDTO");

            migrationBuilder.DropTable(
                name: "SimpleProductDTO");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "SeasonalCollection",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "SeasonalCollection",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomePageSettingsId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "HotDeal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "HotDeal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SettingsId",
                table: "HomePageLayouts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subtitle",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ButtonText",
                table: "HeroCarouselSlide",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "CustomerReview",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CustomerReview",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "CustomerReview",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Avatar",
                table: "CustomerReview",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomePageSettingsId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_HomePageSettingsId",
                table: "Products",
                column: "HomePageSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_HomePageSettingsId",
                table: "Categories",
                column: "HomePageSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_HomePageSettings_HomePageSettingsId",
                table: "Categories",
                column: "HomePageSettingsId",
                principalTable: "HomePageSettings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HomePageLayouts_HomePageSettings_SettingsId",
                table: "HomePageLayouts",
                column: "SettingsId",
                principalTable: "HomePageSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_HomePageSettings_HomePageSettingsId",
                table: "Products",
                column: "HomePageSettingsId",
                principalTable: "HomePageSettings",
                principalColumn: "Id");
        }
    }
}
