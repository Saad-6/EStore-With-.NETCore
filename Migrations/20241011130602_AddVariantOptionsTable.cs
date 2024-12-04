using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EStore.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantOptionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImage_VariantOption_VariantOptionId",
                table: "ProductImage");

            migrationBuilder.DropForeignKey(
                name: "FK_Variant_Products_ProductId",
                table: "Variant");

            migrationBuilder.DropForeignKey(
                name: "FK_VariantOption_Variant_VariantId",
                table: "VariantOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VariantOption",
                table: "VariantOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Variant",
                table: "Variant");

            migrationBuilder.RenameTable(
                name: "VariantOption",
                newName: "VariantOptions");

            migrationBuilder.RenameTable(
                name: "Variant",
                newName: "Variants");

            migrationBuilder.RenameIndex(
                name: "IX_VariantOption_VariantId",
                table: "VariantOptions",
                newName: "IX_VariantOptions_VariantId");

            migrationBuilder.RenameIndex(
                name: "IX_Variant_ProductId",
                table: "Variants",
                newName: "IX_Variants_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VariantOptions",
                table: "VariantOptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Variants",
                table: "Variants",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImage_VariantOptions_VariantOptionId",
                table: "ProductImage",
                column: "VariantOptionId",
                principalTable: "VariantOptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VariantOptions_Variants_VariantId",
                table: "VariantOptions",
                column: "VariantId",
                principalTable: "Variants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImage_VariantOptions_VariantOptionId",
                table: "ProductImage");

            migrationBuilder.DropForeignKey(
                name: "FK_VariantOptions_Variants_VariantId",
                table: "VariantOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Variants_Products_ProductId",
                table: "Variants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Variants",
                table: "Variants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VariantOptions",
                table: "VariantOptions");

            migrationBuilder.RenameTable(
                name: "Variants",
                newName: "Variant");

            migrationBuilder.RenameTable(
                name: "VariantOptions",
                newName: "VariantOption");

            migrationBuilder.RenameIndex(
                name: "IX_Variants_ProductId",
                table: "Variant",
                newName: "IX_Variant_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_VariantOptions_VariantId",
                table: "VariantOption",
                newName: "IX_VariantOption_VariantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Variant",
                table: "Variant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VariantOption",
                table: "VariantOption",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImage_VariantOption_VariantOptionId",
                table: "ProductImage",
                column: "VariantOptionId",
                principalTable: "VariantOption",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Variant_Products_ProductId",
                table: "Variant",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VariantOption_Variant_VariantId",
                table: "VariantOption",
                column: "VariantId",
                principalTable: "Variant",
                principalColumn: "Id");
        }
    }
}
