using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EStore.Migrations
{
    /// <inheritdoc />
    public partial class addedVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SEO_SEOId",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "SEO",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "SEO",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CanonicalUrl",
                table: "SEO",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "SEOId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "VariantOptionId",
                table: "ProductImage",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Variant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Variant_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VariantOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VariantId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantOption_Variant_VariantId",
                        column: x => x.VariantId,
                        principalTable: "Variant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_VariantOptionId",
                table: "ProductImage",
                column: "VariantOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Variant_ProductId",
                table: "Variant",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VariantOption_VariantId",
                table: "VariantOption",
                column: "VariantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImage_VariantOption_VariantOptionId",
                table: "ProductImage",
                column: "VariantOptionId",
                principalTable: "VariantOption",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SEO_SEOId",
                table: "Products",
                column: "SEOId",
                principalTable: "SEO",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImage_VariantOption_VariantOptionId",
                table: "ProductImage");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_SEO_SEOId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "VariantOption");

            migrationBuilder.DropTable(
                name: "Variant");

            migrationBuilder.DropIndex(
                name: "IX_ProductImage_VariantOptionId",
                table: "ProductImage");

            migrationBuilder.DropColumn(
                name: "VariantOptionId",
                table: "ProductImage");

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "SEO",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "SEO",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CanonicalUrl",
                table: "SEO",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SEOId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SEO_SEOId",
                table: "Products",
                column: "SEOId",
                principalTable: "SEO",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
