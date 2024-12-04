using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EStore.Migrations
{
    /// <inheritdoc />
    public partial class addedVariantInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SelectedVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CartItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectedVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectedVariants_CartItems_CartItemId",
                        column: x => x.CartItemId,
                        principalTable: "CartItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SelectedVariants_CartItemId",
                table: "SelectedVariants",
                column: "CartItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SelectedVariants");
        }
    }
}
